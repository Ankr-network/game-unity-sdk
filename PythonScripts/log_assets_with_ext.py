import os
import sys


def log_assets_with_ext(project_path, extension):
    assets_dir = os.path.join(project_path, "Assets")
    for root, dirs, files in os.walk(assets_dir):
        for file in files:
            if file.endswith(extension):
                print(os.path.join(root, file))


if __name__ == "__main__":

    if len(sys.argv) < 2:
        print("Usage: python script.py <extension>")
        sys.exit(1)

    extension = sys.argv[1]

    file_path = os.path.abspath(__file__)
    dir_path = os.path.dirname(file_path)
    parent_dir_path = os.path.abspath(os.path.join(dir_path, os.pardir))

    log_assets_with_ext(parent_dir_path, extension)