This is my take on yet another LED controller for Trimui Smart Pro.

I got tired of seeing the other alternatives out there and they all seem to take ~5%, which I consider unacceptable just to switch up some dumb lights.

This one takes less than 1% of a CPU core (0% most of the time, depending on the effect you choose).

On the other hand, since I wrote this to satisfy my needs, it has a lot of limitations:

* It could work on Trimui Brick but since there are some hardware differences, expect some LEDs to not light up.
* It will work on any OS, but the released scripts are specifically tailored for Knulli

Installation on knulli
=======

1. Download a release
2. Unzip and copy to `/userdata/system/`. You should have folders named `/userdata/system/service` and `/userdata/system/trimui_sharp_led`
3. Go to `System Settings > Services`, disable all other LED services, enable this service TRIMUI_SHARP_LED
