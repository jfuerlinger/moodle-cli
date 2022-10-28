# Moodle CLI

Simple utility to fetch the moodle api.

![MoodleCLIInProgress](./assets/moodle-cli.gif)

## Getting started

Define two Environment variables:

 - MOODLE_USER
 - MOODLE_PASSWORD
 
 Start the CLI without any paramters.
 
## Command line behavior

### Help command 

 ```
 moodle-cli --help
 ```

![HelpCommand](./assets/screenshot_help_command.png)

### List assignments command

```
 moodle-cli.exe assignments list --courseid 3593
 ```

![ListAssignmentsCommand](./assets/screenshot_result_assignments_list_command.png)

 ## Status
 
 [![.NET](https://github.com/jfuerlinger/moodle-cli/actions/workflows/build.yml/badge.svg)](https://github.com/jfuerlinger/moodle-cli/actions/workflows/build.yml)
