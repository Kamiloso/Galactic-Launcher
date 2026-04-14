import subprocess
import threading
import tkinter as tk
from tkinter import ttk, messagebox, filedialog
from datetime import datetime

CONTAINER_NAME = "galactic-mysql"
DB_USER = "root"
DB_PASSWORD = "HardCodedPass!123"
DB_NAME = "galactic"


class DockerGUI:
    def __init__(self, root):
        self.root = root
        self.root.title("MySQL Docker Manager")

        self.busy = False
        self.container_status = "unknown"
        self.show_raw = tk.BooleanVar(value=False)

        self.compose_cmd = self.detect_compose()

        if not self.check_docker():
            messagebox.showerror(
                "Docker error",
                "Docker is not running.\nStart Docker Desktop and try again."
            )
            root.destroy()
            return

        self.create_ui()
        self.update_status_loop()

    # ---------- SYSTEM ----------

    def detect_compose(self):
        try:
            subprocess.run(["docker", "compose", "version"], stdout=subprocess.DEVNULL)
            return ["docker", "compose"]
        except:
            return ["docker-compose"]

    def check_docker(self):
        try:
            subprocess.run(["docker", "info"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            return True
        except:
            return False

    def get_container_status(self):
        try:
            result = subprocess.run(
                ["docker", "inspect", "-f", "{{.State.Status}}", CONTAINER_NAME],
                capture_output=True,
                text=True
            )
            if result.returncode != 0:
                return "not_found"
            return result.stdout.strip()
        except:
            return "error"

    # ---------- UI ----------

    def create_ui(self):
        style = ttk.Style()
        style.configure(".", font=("Segoe UI", 11))

        root_frame = ttk.Frame(self.root, padding=15)
        root_frame.pack(fill="both", expand=True)

        # STATUS
        status_frame = ttk.LabelFrame(root_frame, text="Status", padding=10)
        status_frame.pack(fill="x", pady=5)

        self.status_var = tk.StringVar(value="Checking...")
        self.status_label = ttk.Label(status_frame, textvariable=self.status_var, font=("Segoe UI", 13, "bold"))
        self.status_label.pack(anchor="w")

        # ACTIONS
        action_frame = ttk.LabelFrame(root_frame, text="Actions", padding=10)
        action_frame.pack(fill="x", pady=5)

        self.btn_start = ttk.Button(action_frame, text="Start", command=self.start)
        self.btn_stop = ttk.Button(action_frame, text="Stop", command=self.stop)
        self.btn_reset = ttk.Button(action_frame, text="Reset (wipe + recreate)", command=self.reset)
        self.btn_purge = ttk.Button(action_frame, text="NUKE (delete everything)", command=self.nuke)
        self.btn_console = ttk.Button(action_frame, text="Open MySQL console", command=self.open_console)
        self.btn_sql = ttk.Button(action_frame, text="Run SQL file", command=self.run_sql_file)

        self.btn_start.grid(row=0, column=0, padx=5, pady=5)
        self.btn_stop.grid(row=0, column=1, padx=5, pady=5)
        self.btn_reset.grid(row=0, column=2, padx=5, pady=5)
        self.btn_purge.grid(row=0, column=3, padx=5, pady=5)
        self.btn_console.grid(row=1, column=0, padx=5, pady=5)
        self.btn_sql.grid(row=1, column=1, padx=5, pady=5)

        # LOGS
        log_frame = ttk.LabelFrame(root_frame, text="Logs", padding=10)
        log_frame.pack(fill="both", expand=True, pady=5)

        ttk.Checkbutton(
            log_frame,
            text="Show raw Docker output (debug)",
            variable=self.show_raw
        ).pack(anchor="w", pady=(0, 5))

        self.log = tk.Text(log_frame, height=15, state="disabled")
        self.log.pack(fill="both", expand=True)

    # ---------- LOGGING ----------

    def log_write(self, text):
        timestamp = datetime.now().strftime("[%H:%M:%S] ")
        self.log.configure(state="normal")
        self.log.insert(tk.END, timestamp + text + "\n")
        self.log.configure(state="disabled")
        self.log.see(tk.END)

    # ---------- STATE ----------

    def set_busy(self, state):
        self.busy = state
        self.update_buttons()

    def update_buttons(self):
        running = self.container_status == "running"

        self.btn_start["state"] = "normal" if not running and not self.busy else "disabled"
        self.btn_stop["state"] = "normal" if running and not self.busy else "disabled"
        self.btn_reset["state"] = "normal" if not self.busy else "disabled"
        self.btn_purge["state"] = "normal" if not self.busy else "disabled"

        self.btn_console["state"] = "normal" if running and not self.busy else "disabled"
        self.btn_sql["state"] = "normal" if running and not self.busy else "disabled"

    def update_status_loop(self):
        if not self.check_docker():
            self.status_var.set("Docker not available")
            self.status_label.configure(foreground="red")
            self.update_buttons()
            self.root.after(2000, self.update_status_loop)
            return

        status = self.get_container_status()
        self.container_status = status

        if status == "running":
            self.status_var.set("Container RUNNING")
            self.status_label.configure(foreground="green")
        elif status == "exited":
            self.status_var.set("Container STOPPED")
            self.status_label.configure(foreground="orange")
        elif status == "not_found":
            self.status_var.set("Container NOT CREATED")
            self.status_label.configure(foreground="gray")
        else:
            self.status_var.set("Unknown state")
            self.status_label.configure(foreground="red")

        self.update_buttons()
        self.root.after(2000, self.update_status_loop)

    # ---------- EXEC ----------

    def run_action(self, label, cmd):
        if self.busy:
            return

        def task():
            self.set_busy(True)
            self.log_write(label + "...")

            try:
                if isinstance(cmd, str):
                    result = subprocess.run(cmd, capture_output=True, text=True, shell=True)
                else:
                    result = subprocess.run(cmd, capture_output=True, text=True)

                if result.returncode == 0:
                    self.log_write(label + " ✓")
                else:
                    self.log_write(label + " FAILED")

                if self.show_raw.get():
                    if result.stdout:
                        self.log_write(result.stdout.strip())
                    if result.stderr:
                        self.log_write(result.stderr.strip())

            except Exception as e:
                self.log_write(f"{label} ERROR: {e}")

            self.set_busy(False)

        threading.Thread(target=task, daemon=True).start()

    # ---------- ACTIONS ----------

    def start(self):
        self.run_action("Starting container", self.compose_cmd + ["up", "-d"])

    def stop(self):
        self.run_action("Stopping container", self.compose_cmd + ["stop"])

    def reset(self):
        if not messagebox.askyesno("Confirm", "Reset container and DELETE all data?"):
            return

        def task():
            self.set_busy(True)
            self.log_write("Resetting environment...")
            subprocess.run(self.compose_cmd + ["down", "-v"])
            subprocess.run(self.compose_cmd + ["up", "-d"])
            self.log_write("Environment reset complete ✓")
            self.set_busy(False)

        threading.Thread(target=task, daemon=True).start()

    def nuke(self):
        if not messagebox.askyesno("Confirm", "This will DELETE EVERYTHING and NOT restart. Continue?"):
            return

        self.run_action(
            "NUKE: removing all containers, volumes, and orphans",
            self.compose_cmd + ["down", "-v", "--remove-orphans"]
        )

    def open_console(self):
        if self.container_status != "running":
            messagebox.showwarning("Unavailable", "Container must be running.")
            return

        subprocess.Popen(
            f'start cmd /k docker exec -it {CONTAINER_NAME} mysql -u{DB_USER} -p{DB_PASSWORD} {DB_NAME}',
            shell=True
        )

    def run_sql_file(self):
        if self.container_status != "running":
            messagebox.showwarning("Unavailable", "Container must be running.")
            return

        path = filedialog.askopenfilename(filetypes=[("SQL files", "*.sql")])
        if not path:
            return

        cmd = f'docker exec -i {CONTAINER_NAME} mysql -u{DB_USER} -p{DB_PASSWORD} {DB_NAME} < "{path}"'
        self.run_action("Executing SQL file", cmd)


if __name__ == "__main__":
    root = tk.Tk()
    root.geometry("760x540")
    app = DockerGUI(root)
    root.mainloop()