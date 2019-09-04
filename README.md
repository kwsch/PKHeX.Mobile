PKHeX
=====
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)

Pokémon core series save editor for Mobile Devices (Android/iOS), programmed in [C#](https://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29).

Supports the following files:
* Save files ("main", \*.sav, \*.dsv, \*.dat, \*.gci, \*.bin)
* Individual Pokémon entity files (.pk\*, \*.ck3, \*.xk3, \*.bk4, \*.pb7)
* Transferring from one generation to another, converting formats along the way.

Data is displayed in a view which can be edited and saved.

**We do not support or condone cheating at the expense of others. Do not use significantly hacked Pokémon in battle or in trades with those who are unaware hacked Pokémon are in use.**

## Building

PKHeX.Mobile is a Xamarin Forms application which requires [Xamarin Forms 4.0](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/release-notes/4.0/4.0.0).

Customized Xamarin UI controls are provided by [Syncfusion](https://www.syncfusion.com/products/communitylicense) (community license).

Android/iOS operating system version requirements TBD.

Having trouble building the app? Be sure to hit Clean & Rebuild All before trying to deploy to your device.

## Dependencies

TBD

## Installation

Build->Clean Solution, Build->Rebuild All, Build->Deploy

On your device, manually grant the permissions required (storage, camera) so that files can be written & the QR scanning can be performed without issue.

Once you have granted the app permissions, you can run it!