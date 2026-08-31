/* ============================================================
   RACEDAY SYSTEM - PART 1
   DATABASE SQL SCRIPT
   ============================================================

   Purpose:
   This script creates the RaceDay database and the tables
   required for the RaceDay event management system.

   The database supports:
   - Users with Organiser and Participant roles
   - Running, walking and cycling events
   - Event categories
   - Participant enrolments
   - Race results

   NOTE:
   Passwords must be hashed by the application before they
   are stored in the PASSWORD_HASH column. The sample values
   below are placeholders for demonstration purposes.
   ============================================================ */


/* ============================================================
   CREATE DATABASE
   ============================================================

   Creates the RaceDay database that will store all information
   related to users, events, categories, enrolments and results.
   ============================================================ */

CREATE DATABASE RACEDAY_POE;
USE RACEDAY_POE;