# TerraCiia

A Terraria clone made in C# using godot.

<br>

[Insert pictures here when the game is more advanced]

<br>

## Table of contents

- [Features](#features)
- [Run the Project](#run-the-project)
- [Contributing](#contributing)
    - [Guidelines](#guidelines)


<br>

## Features

Nothing for the moment

<br>

## Run the Project

First you need to download the last .NET version of the [Godot Engine](https://godotengine.org/download/windows/).

And [.NET](https://dotnet.microsoft.com/en-us/download).

Then you can just clone the project and open it with Godot.

If you can't see the build button next to the run project button on the top-right corner, you can generate the C# solution in Project -> Tools -> C# -> Create C# solution.

See [the official documentation](https://docs.godotengine.org/en/stable/) for additionals informations.

<br>

## Contributing

This part was taken from https://github.com/c2i-junia/rustcraft?tab=readme-ov-file#contributing

### Guidelines

#### Commit messages:

- We follow the [Conventional Commit specification](https://www.conventionalcommits.org/en/v1.0.0/). Our commit types are inspired by the [Karma specification](http://karma-runner.github.io/6.4/dev/git-commit-msg.html)
  ```
  <type>[optional scope]: <description>

  [optional body]

  [optional footer(s)]
  ```

  Allowed <type> values: 
  - **feat** for a new feature for the user, not a new feature for build script. Such commit will trigger a release bumping a MINOR version.
  - **fix** for a bug fix for the user, not a fix to a build script. Such commit will trigger a release bumping a PATCH version.
  - **perf** for performance improvements. Such commit will trigger a release bumping a PATCH version.
  - **docs** for changes to the documentation.
  - **style** for formatting changes, missing semicolons, etc.
  - **refactor** for refactoring production code, e.g. renaming a variable.
  - **test** for adding missing tests, refactoring tests; no production code change.
  - **build** for updating build configuration, development tools or other changes irrelevant to the user.

- Write commit messages in the present tense (e.g., "Add feature X" instead of "Added feature X").

#### Branches
- Use the naming convention `<type>/<name>` for branches introducing new features. Only use lowercase letters, numbers, and dashes.
- The `main` branch should always compile successfully and be free of warnings.
- Experimental branches are allowed to include code that does not build successfully.

<br>
