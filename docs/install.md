# Installation

Use the following command to install the
[NSec.Cryptography NuGet package](https://www.nuget.org/packages/NSec.Cryptography/23.9.0-preview.3):

    $ dotnet add package NSec.Cryptography --version 23.9.0-preview.3


## Supported Platforms

[NSec 23.9.0-preview.3](https://www.nuget.org/packages/NSec.Cryptography/23.9.0-preview.3)
is intended to run on all
[supported versions of .NET](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
on the following platforms:

|                       | `-x64`   | `-x86`   | `-arm64` | `-arm`   |
|:----------------------|:--------:|:--------:|:--------:|:--------:|
| **`win-`**            | &check;  | &check;  |          |          |
| **`linux-`**          | &check;  |          | &check;  | &check;  |
| **`linux-musl-`**     | &check;  |          | &check;  | &check;  |
| **`osx-`**            | &check;  |          | &check;  |          |
| **`ios-`**            |          |          |          |          |
| **`android-`**        |          |          |          |          |


Please note:

1. For Windows, the
   [Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017, 2019, and 2022](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads)
   is required. This is part of the .NET SDK but might not be present on a
   clean Windows installation.

2. The AES-GCM implementation in NSec is hardware-accelerated and may not be
   available on all architectures. Support can be determined at runtime using
   the static `IsSupported` property of the `NSec.Cryptography.Aes256Gcm` class.


## Tested Platforms

[NSec 23.9.0-preview.3](https://www.nuget.org/packages/NSec.Cryptography/23.9.0-preview.3)
has been tested to run on the following platforms and .NET versions:

| OS                   | Version  | Architectures | .NET            |
|:-------------------- |:-------- |:------------- |:--------------- |
| Windows 11           | 22H2     | x64           | 7.0.11 / 6.0.22 |
| Windows Server       | 2022     | x64           | 7.0.11 / 6.0.22 |
|                      |          |               |                 |
| macOS                | 11.7     | x64           | 7.0.11 / 6.0.22 |
|                      | 12.6     | x64           | 7.0.11 / 6.0.22 |
|                      | 13.4     | x64           | 7.0.11 / 6.0.22 |
|                      |          |               |                 |
| Alpine Linux         | 3.17     | x64           | 7.0.10          |
|                      | 3.18     | x64           | 7.0.11          |
| CentOS Linux         | 7        | x64           | 7.0.11 / 6.0.22 |
| Debian               | 10       | x64           | 7.0.11 / 6.0.22 |
|                      | 11       | x64           | 7.0.11 / 6.0.22 |
|                      | 12       | x64           | 7.0.11 / 6.0.22 |
| Fedora               | 37       | x64           | 7.0.11 / 6.0.22 |
|                      | 38       | x64           | 7.0.11 / 6.0.22 |
| Ubuntu               | 16.04    | x64           | 7.0.11 / 6.0.22 |
|                      | 18.04    | x64           | 7.0.11 / 6.0.22 |
|                      | 20.04    | x64           | 7.0.11 / 6.0.22 |
|                      | 22.04    | x64           | 7.0.11 / 6.0.22 |

The other supported platforms should work as well, but haven't been tested.


## Frequently Asked Questions

Below are some frequently asked questions:

**Q**: What could cause a *System.DllNotFoundException: Unable to load shared
library 'libsodium' or one of its dependencies.* when using the
NSec.Cryptography NuGet package?  
**A**: This exception can occur if the operating system or architecture is not
supported, or if the Visual C++ Redistributable has not been installed on a
Windows system. Please refer to the [Supported Platforms](#supported-platforms)
section above.
