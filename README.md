<h1 align="center">SuperManager</h1>

<div align="center">
  <strong>Custom file manager</strong>
</div>
<div align="center">
  A file manager which contains way more functionality than usual one
</div>

<br />

<div align="center">
  <!-- Build Status -->
  <a href="https://ci.appveyor.com/project/SmirnovAlexander/file-manager">
    <img src="https://ci.appveyor.com/api/projects/status/t3qfpiuo0vivctb9?svg=true"
      alt="Build Status" />
  </a>
</div>

<div align="center">
  <sub>Built with ❤︎ by
  <a href="https://github.com/SmirnovAlexander">
    Smirnov Alexander
  </a>
</div>

## Table of Contents
- [Introduction](#introduction)
- [How it looks like](#how-it-looks-like)
- [What is inside](#what-is-inside)
- [See Also](#see-also)

## Introduction
This manager was made on programming lessons during the 2nd and 3rd semesters.

It illustrates a bunch of programming patterns and optimization techniques.

## How it looks like
* That is how main screen looks like:
![image](https://user-images.githubusercontent.com/32129186/54601313-c9cc6c00-4a4f-11e9-9c30-b0a77a95fc83.png)

* You are able to search throw files using masks:
![image](https://user-images.githubusercontent.com/32129186/55531976-d0e0b480-56b4-11e9-9cdc-7860e81d1d88.png)

* Get a .txt file statistics:
![image](https://user-images.githubusercontent.com/32129186/55532159-8b70b700-56b5-11e9-89cf-d4b2b4b89b88.png)



## What is inside
In this section provided list of all functions of this manager:

### Main functionality
- Disks, files and folders navigation
- Copying and moving files and folders
- Name changing, deletion
- Archiving
- Login & password (serializing)
- Searching for personal data via regex patterns
- File downloading with cancelling

### Working with treads, task, async
- Treading search and archiving
- Search and archiving via Parallels.ForEach
- Search and archiving via Tasks
- Searching for file names and folders in currrent directory with masks
- Search and archiving via async/await
- Async download manager with progress

### Patterns
- Strategy pattern to different way of searching data
- Template method to archiving
- Visitor pattern to encryption data with key
- Visitor pattern to calculate MD5-hash
- Decorator pattern to isolate manager from System.IO classes
- MVP pattern to tear dependicies between logic and forms

## See Also
- [MemDer](https://github.com/SmirnovAlexander/MemDer) - andriod meme application 
- [4th semester homeworks](https://github.com/SmirnovAlexander/homeworks_3rdsem) - interesting F# tasks 

## License
[MIT](https://tldrlegal.com/license/mit-license)
