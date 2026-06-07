import os
from utils.const import *

class ChoiceMenu:
    def __init__(self, title: str, options: list[str]):
        self.title = title
        self.options = options

    def display(self) -> int:
        print("\n" + self.title)
        
        for i, option in enumerate(self.options, start=1):
            print(f"{i}. {option}")

        while True:
            try:
                choice = int(input("> "))
                if choice < 1 or choice > len(self.options):
                    raise ValueError
                
                return choice
            
            except ValueError:
                print(f"Invalid choice! Enter a number between 1 and {len(self.options)}.")

class InputMenu:
    def __init__(self, title: str, questions: list[str]):
        self.title = title
        self.questions = questions

    def display(self) -> list[str]:
        print("\n" + self.title)

        answers = []
        for question in self.questions:
            answer = input(f"{question}\n> ")
            answers.append(answer)

        return answers