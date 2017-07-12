# list-size-win

[![Build Status](https://travis-ci.org/onkbear/list-size-win.svg?branch=master)](https://travis-ci.org/onkbear/list-size-win)

List directory contents size

## Installation

* Download the latest `lss.exe` from [here](https://github.com/onkbear/list-size-win/releases).
* Add a directory that existing `lss.exe` to the `PATH` environment variable.

## Usage

```
> lss [OPTION] [FILE]
```

* OPTION
  * `-h`, `--help` Display this help and exit.
  * `-s`, `--sort` Sort by file size.
* FILE
  * File path

## Example

```
c:\Users\onkbear>lss --sort
Type              Size Occupancy File
D       17,040,120,564    69.91% VirtualBox VMs
D        3,927,892,657    16.12% github
D        2,499,682,936    10.26% Downloads
D          606,041,105     2.49% AppData
D           64,079,458     0.26% .atom
D           54,320,494     0.22% .electron
D           32,765,206     0.13% go
D           23,591,374     0.10% Pictures
F           22,020,094     0.09% NTUSER.DAT
D           17,530,180     0.07% Music
D           12,738,570     0.05% Tracing
D           11,761,183     0.05% Documents
D            5,973,601     0.02% Desktop
D            5,955,917     0.02% .vscode
F            5,179,396     0.02% ntuser.dat.LOG2
F            5,179,396     0.02% ntuser.dat.LOG1
D            2,691,644     0.01% .nuget
D            2,571,558     0.01% .node-gyp
```

## Development

### Build

```
MsBuild /p:Configuration=Release ListSizeWin.sln
```

## License

MIT
