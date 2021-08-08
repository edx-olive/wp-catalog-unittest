#!/bin/bash
Xvfb :99.0 &
export DISPLAY=:99.0
./CampusHomePage