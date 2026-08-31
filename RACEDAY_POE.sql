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

/* ============================================================
   1. USER TABLE
   ============================================================
   Stores information about all users of the RaceDay system.
   Organisers and Participants are NOT stored in separate
   tables. Instead, both are stored as users and their role
   is identified using the ROLE column.
   ROLE can only contain:
   - Organiser
   - Participant
   */

CREATE TABLE [USER]
(
    USER_ID INT IDENTITY(1,1) PRIMARY KEY,
    FIRST_NAME VARCHAR(50) NOT NULL,
    LAST_NAME VARCHAR(50) NOT NULL,
    -- UNIQUE prevents two users from registering with the same email address.
    EMAIL VARCHAR(100) NOT NULL UNIQUE,
    PHONE_NUMBER VARCHAR(20),
    -- Stores the hashed version of the user's password. The original/plain-text password must NOT be stored.
    PASSWORD_HASH VARCHAR(255) NOT NULL,
    -- Determines whether the user is an Organiser or a Participant.
    ROLE VARCHAR(20) NOT NULL,
    --NULL Because when someone first registers, they might not have uploaded a profile picture yet. A user can exist even if they don't have a profile picture yet.
    PROFILE_PICTURE_URL VARCHAR(500) NULL,

    -- Ensures that only the two approved roles can be stored in the ROLE column.
    CONSTRAINT CK_USER_ROLE
        CHECK (ROLE IN ('Organiser', 'Participant'))
);

SELECT * FROM [USER];

