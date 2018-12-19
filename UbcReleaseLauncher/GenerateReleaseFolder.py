import argparse
import os
import zipfile
import shutil
import GenerateManifest


ubc_folder = "UnofficialBestiaryCompanion"
ubc_files = ["Bestiary.exe", "log4net.dll"]
launcher_folder = "UBCLauncher"
launcher_files = ["BestiaryLauncher.exe", "log4net.dll", "Newtonsoft.Json.dll"]
launcher_images = "LauncherImages"
images = "Images"
icons = "Icons"
familiar_data = "FamiliarData"
display_icons = "DisplayIcons"
view_icons = "ViewIcons"


def clear_folder(folder):
    for file in os.listdir(folder):
        file_path = os.path.join(folder, file)
        try:
            if os.path.isfile(file_path):
                os.unlink(file_path)
        except Exception as e:
            print(e)


def generate_zip_of_files(zip_name, root_folder, contents):
    gen_zip = zipfile.ZipFile(zip_name + '.zip', mode='w')
    try:
        for item in contents:
            gen_zip.write(os.path.join(root_folder, item), item)
    finally:
        gen_zip.close()
    return gen_zip


def main():
    parser = argparse.ArgumentParser(
        description="utility for generating manifests for UBC"
    )
    parser.add_argument(
        'output_folder', help='directory to place release assets into'
    )
    parser.add_argument(
        'solution_folder', help='root path for files that need to go into release assets'
    )
    args = parser.parse_args()

    bestiary_resources = os.path.join(args.solution_folder, "Bestiary\\Resources")
    bestiary_main = os.path.join(args.solution_folder, "Bestiary\\bin\\Release")
    bestiary_launcher = os.path.join(args.solution_folder, "BestiaryLauncher\\bin\\Release")
    bestiary_launcher_resources = os.path.join(bestiary_launcher, "Resources")

    clear_folder(args.output_folder)

    shutil.make_archive(launcher_images, 'zip', bestiary_launcher_resources, launcher_images)
    shutil.move(launcher_images + ".zip", args.output_folder)
    shutil.make_archive(familiar_data, 'zip', bestiary_resources, familiar_data)
    shutil.move(familiar_data + ".zip", args.output_folder)
    shutil.make_archive(icons, 'zip', bestiary_resources, icons)
    shutil.move(icons + ".zip", args.output_folder)
    shutil.make_archive(images, 'zip', bestiary_resources, images)
    shutil.move(images + ".zip", args.output_folder)
    shutil.make_archive(display_icons, 'zip', bestiary_resources, display_icons)
    shutil.move(display_icons + ".zip", args.output_folder)
    shutil.make_archive(view_icons, 'zip', bestiary_resources, view_icons)
    shutil.move(view_icons + ".zip", args.output_folder)

    generate_zip_of_files(launcher_folder, bestiary_launcher, launcher_files)
    shutil.move(launcher_folder + ".zip", args.output_folder)
    generate_zip_of_files(ubc_folder, bestiary_main, ubc_files)
    shutil.move(ubc_folder + ".zip", args.output_folder)

    GenerateManifest.generate_manifest(args.output_folder)


if __name__ == "__main__":
    main()
