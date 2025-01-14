#!/usr/bin/env bash
set -e

check_and_install_dotnet() {
    echo "Checking if .NET is installed..."

    if command -v dotnet &> /dev/null
    then
        installed_version="$(dotnet --version)"
        echo "Detected .NET version: $installed_version"

        major_installed="$(echo "$installed_version" | cut -d '.' -f1)"
        required_major=8

        if [ "$major_installed" -ge "$required_major" ]; then
            echo ".NET $installed_version is already up to date, no upgrade needed."
        else
            read -r -p ".NET $installed_version is installed, which is older than 8.0. Do you want to upgrade? [y/N] " confirm
            if [ "$confirm" = "y" ] || [ "$confirm" = "Y" ]; then
                install_or_upgrade_dotnet
            else
                echo "Skipping upgrade."
            fi
        fi
    else
        echo ".NET is not installed. Installing now..."
        install_or_upgrade_dotnet
    fi
}

install_or_upgrade_dotnet() {
    sudo apt-get update
    sudo apt-get install -y wget

    if [ ! -f packages-microsoft-prod.deb ]; then
        wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    fi

    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update
    sudo apt-get install -y apt-transport-https
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-8.0
    echo "Dotnet 8.0 has been installed/upgraded successfully."
}

check_and_install_dotnet