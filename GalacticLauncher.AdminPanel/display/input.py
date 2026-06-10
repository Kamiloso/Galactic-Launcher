from errors.abort_error import AbortError


def input_string(prompt: str) -> str:
    print(f"\n{prompt}")
    choice = input("> ")

    if choice == "/exit": # Universal exit command
        raise AbortError("User aborted input", is_user_abort=True)
    
    return choice


def input_number(prompt: str, range: tuple[int, int]) -> int:
    r_min = range[0]
    r_max = range[1]

    choice = input_string(prompt)
    
    try:
        n = int(choice)
        if r_min > n or n > r_max:
            raise ValueError
        return n
    
    except ValueError:
        raise AbortError(f"Invalid input. Must be integer between {r_min} and {r_max}.")