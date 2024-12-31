# Playnite.YearInReview
Celebrate your last year of play by reviewing some of the play statistics from Playnite GameActivity plugin!

## Contributing
The structure of the code is still in limbo, but here's the general idea:
* Extensions - contains other extension related code. In this case - GameActivity plugin.
* Infrastructure - report agnostic code. Some of the code might still need to move.
* Model - report specific code.
  * Aggregators - aggregators are the data processing classes. They take GameActivity extension and Playnite data to provide certain aggregated data that will be later used to format a report.
  * Filters - filters generally trim down the data in one way or another.
  * Reports - final report code.
    * 1970 - all reports starting from year 1970. More details below.
      * MVVM - unconventionally, all view models and views live in one place.
    * Composer - something that takes all aggregator data, and converts it into a report data.
    * Report - this is the class that holds all report information. It will be serialized and stored on disk, or shared between people.
  * MVVM - generic MVVM stuff (non-report specific).
  * ReportManager - this should handle all the report loading / generating. Work in progress.
* Settings - all the settings stuff.

### What about the year prefix / suffix?
Since all reports are shareable, it's vital the Report.cs class does not change after it has been used and serialized. While the initial organization is messy, the idea that every part of the report is in a self-contained set of classes. What this also means, is that once we want to change the report structure or add more data, we can easily do it by creating new classes with the new year suffix. As in:
* 1970 - all years until 2026.
* 2026 - let's say we create a new format for 2026. Then all reports starting from 2026 will use these classes.

### Unit Testing
Most of the code should be written with Unit Tests. It is preferable to apply TDD.