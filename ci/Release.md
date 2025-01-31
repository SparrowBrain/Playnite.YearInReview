This document describes release process.

Pre-requisites:
* GitHub CLI tools. Get them here: https://cli.github.com/. Add to your PATH environment variable.
* Playnite Toolkit. Installed with Playnite: https://playnite.link/.
* MSBuild. Installed with Visual Studio.

Double check the paths to executables in Program.cs.

Release flow:

1. Add changes to the `Changelog.txt` file in the format at the top of the file:
```
v0.0.0
- Change 1
- Change 2
``` 
2. Build the solution in Debug mode
3. Run `release.bat`


In case GitHub authentication fails set it up. Either run `gh auth login` or setup an environment variable `GITHUB_TOKEN`. More info: https://cli.github.com/manual/.