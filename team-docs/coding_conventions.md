# Conventions
- [Coding Guideline](#coding-guideline)
  - [Variable Names](#variable-names)
  - [Indentation](#indentation)
  - [File Names](#file-names)
  - [File Locations](#file-locations)
  - [Commenting Guidelines](#commenting-guidelines)
- [Git Guidelines](#git-guidelines)
  - [Branches](#branches)
  - [Pull Requests](#pull-requests)

## Coding Guideline

- ### Variable Names
  - Use camelCase naming convention when defining variables (eg. playerMoveForward instead of PlayerMoveForward).
    - The Exception to this is when defining a Function, Class or Widget. Always use capitalized letters for each word in this case (eg. MainMenu(), FindMax() etc).
  - Use an underscore at the beginning of a booleans name when defining the variable.
  - Use an underscore at the beginning of a variables name when defining the variable in a provider class.
  - Avoid using an underscore when defining a variable anywhere else.
  - Use meaningful, self describing variable names in all cases.
    - Avoid using shortened words for variable names as it makes the code harder to read and understand for other developers. (eg. plyrmovectrl should be PlayerMovementController).

- ### Indentation
  - Use a tab size of two spaces when coding.

- ### File Names
  - When naming a file make sure to use meaningful names without shortened words.
  - Use underscores instead of spaces in the names.
  - When naming a file use the story point name is for at the start of the file name. This will make it easier to find the page you're after when looking for them. (eg. movement.cs should be player_movement.cs).

- ### File Locations
  - Files should be in the appropriate folders (eg. a movement function inside the player folder, a menu inside the menu folder etc).

- ### Commenting Guidelines
  - When Commenting something that needs done in future please use "//todo" before the commented task.

## Git Guidelines

- ### Branches
  - When Creating a branch please name it after the story point you're working on.
  - Remember to do a pull before starting any work and before pushing your work.
  - Avoid editing files that are not needed for the current Trello task while on your Branch.

- ### Pull Requests
  - When reviewing a pull request make sure to read over the changed code and verify the variable names follow the guidelines and that the code works.