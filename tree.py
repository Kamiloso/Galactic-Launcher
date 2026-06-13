import sys
from pathlib import Path

TARGET_EXTENSIONS = [".cs", ".py"]
EXCLUDED_DIRS = ["obj"]

class TreeNode:
    def __init__(self, name):
        self.name = name
        self.children = {}
        self.is_file = False

def build_tree(root_path):
    root = Path(root_path).resolve()
    root_node = TreeNode(root.name)
    found_any = False
    
    for file_path in root.rglob("*"):
        if not file_path.is_file() or file_path.suffix not in TARGET_EXTENSIONS:
            continue
            
        rel_path = file_path.relative_to(root)
        
        if any(excluded in rel_path.parts for excluded in EXCLUDED_DIRS):
            continue
            
        found_any = True
        current = root_node
        for part in rel_path.parts:
            if part not in current.children:
                current.children[part] = TreeNode(part)
            current = current.children[part]
        current.is_file = True
        
    return root_node if found_any else None

def print_tree(node, prefix=""):
    children = list(node.children.values())
    children.sort(key=lambda x: (x.is_file, x.name.lower()))
    
    for i, child in enumerate(children):
        is_last = (i == len(children) - 1)
        
        print(f"{prefix}|-- {child.name}")
        
        if not child.is_file:
            new_prefix = prefix + ("    " if is_last else "|   ")
            print_tree(child, new_prefix)

if __name__ == "__main__":
    target = sys.argv[1] if len(sys.argv) > 1 else "."
    target_path = Path(target).resolve()
    
    print(target_path.name)
    tree = build_tree(target_path)
    
    if tree and tree.children:
        print("|")
        print_tree(tree)
    else:
        print("No files found.")