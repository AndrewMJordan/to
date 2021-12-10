# To

> Navigate anywhere on the internet from a terminal

![badge](https://img.shields.io/github/v/tag/andtechstudios/to)

# Installation
> Check out installation instructions [here](https://gitlab.com/andtech/pkg)

# Setup
## Create a hotspot list
### Method 1: Hotspot file in home directory
1. Create a hotspot file at `~/to.json`.

### Method 2: List of hotspot directories
1. Set the environment variable `ANDTECH_TO_PATH` as a delimited[^1] list of directories to search for hotspot files. *To* will read all json files in these directories.

# Usage
* Enter `to` followed by a search query. The best hotspot will be selected and will launch the url in a web browser.

```
$ to github andtech studios
https://github.com/andtechstudios
```

[^1]: Windows users should use a semi-colon (`;`) delimiter. All other users should use a colon (`:`) delimiter.
