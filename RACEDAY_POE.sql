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



/* ============================================================
   2. EVENT TABLE
   ============================================================
   Stores information about events created by Organisers.
   Each event belongs to one Organiser.
   Organiser is also a User in the system. The ROLE column
   identifies whether that user is an Organiser.
   */

CREATE TABLE EVENT
(
    EVENT_ID INT IDENTITY(1,1) PRIMARY KEY,
    -- Identifies the User who created/owns the event. This references USER.USER_ID.
    ORGANISER_ID INT NOT NULL,
    EVENT_NAME VARCHAR(100) NOT NULL,
    DESCRIPTION VARCHAR(500),
    EVENT_DATE DATE NOT NULL,
    LOCATION VARCHAR(200) NOT NULL,
    DISTANCE DECIMAL(6,2) NOT NULL,
    EVENT_TYPE VARCHAR(20) NOT NULL,

    -- Creates the relationship between EVENT and USER. ORGANISER_ID must exist as a USER_ID in USER.
    CONSTRAINT FK_EVENT_ORGANISER
        FOREIGN KEY (ORGANISER_ID)
        REFERENCES [USER](USER_ID),

    -- Restricts event types to the three types required by the RaceDay system.
    CONSTRAINT CK_EVENT_TYPE
        CHECK (EVENT_TYPE IN ('Run', 'Walk', 'Cycle'))
);

SELECT * FROM EVENT;


/* ============================================================
   3. CATEGORY TABLE
   ============================================================
   Stores the categories available for each event.
   */

CREATE TABLE CATEGORY
(
    CATEGORY_ID INT IDENTITY(1,1) PRIMARY KEY,
    EVENT_ID INT NOT NULL,
    CATEGORY_NAME VARCHAR(100) NOT NULL,
    DESCRIPTION VARCHAR(255),

    -- Creates the relationship between CATEGORY and EVENT.
    CONSTRAINT FK_CATEGORY_EVENT
        FOREIGN KEY (EVENT_ID)
        REFERENCES EVENT(EVENT_ID)

        -- If an event is deleted, its categories areautomatically deleted as well.
        ON DELETE CASCADE
);

SELECT * FROM CATEGORY;


/* ============================================================
   4. ENROLMENT TABLE
   ============================================================
   Records when a Participant enters an Event.
 An enrolment connects:
   - A Participant
   - An Event
   - A selected Category
PARTICIPANT_ID refers to USER_ID because a Participant
   is also a User. The USER.ROLE column identifies the user
   as a Participant.
   ============================================================ */

CREATE TABLE ENROLMENT
(
    ENROLMENT_ID INT IDENTITY(1,1) PRIMARY KEY,
    -- Identifies the Participant who entered the event.
    PARTICIPANT_ID INT NOT NULL,
    -- Identifies the Event the Participant entered.
    EVENT_ID INT NOT NULL,
    -- Identifies the Category selected by the Participant.
    CATEGORY_ID INT NOT NULL,
    -- Shows the current status of the enrolment. New enrolments are Pending by default.
    ENROLMENT_STATUS VARCHAR(20) NOT NULL DEFAULT 'Pending',
    -- Automatically records when the enrolment was created.
    ENROLMENT_DATE DATETIME NOT NULL DEFAULT GETDATE(),

    -- Links the Participant to the USER table.
    CONSTRAINT FK_ENROLMENT_PARTICIPANT
        FOREIGN KEY (PARTICIPANT_ID)
        REFERENCES [USER](USER_ID),

    -- Links the enrolment to an EVENT.
    CONSTRAINT FK_ENROLMENT_EVENT
        FOREIGN KEY (EVENT_ID)
        REFERENCES EVENT(EVENT_ID),

    -- Links the enrolment to the selected CATEGORY.
    CONSTRAINT FK_ENROLMENT_CATEGORY
        FOREIGN KEY (CATEGORY_ID)
        REFERENCES CATEGORY(CATEGORY_ID),

    -- Restricts enrolment status to the approved values.
    CONSTRAINT CK_ENROLMENT_STATUS
        CHECK (ENROLMENT_STATUS IN
        ('Pending', 'Confirmed', 'Cancelled'))
);

SELECT * FROM ENROLMENT;

/* ============================================================
   5. RESULT TABLE
   ============================================================
   Stores the result achieved by a Participant after
   completing an event.
   The result is connected to an ENROLMENT because the
   Participant must first be enrolled in an event before
   receiving a result.
   ============================================================ */

CREATE TABLE RESULT
(
    RESULT_ID INT IDENTITY(1,1) PRIMARY KEY,
    ENROLMENT_ID INT NOT NULL,
    FINISH_TIME TIME NOT NULL,
    FINISHING_POSITION INT NOT NULL,

    -- Links the result to the Participant's enrolment.
    CONSTRAINT FK_RESULT_ENROLMENT
        FOREIGN KEY (ENROLMENT_ID)
        REFERENCES ENROLMENT(ENROLMENT_ID),

    -- Ensures that finishing position cannot be zero or a negative number.
    CONSTRAINT CK_RESULT_POSITION
        CHECK (FINISHING_POSITION > 0)
);

SELECT * FROM RESULT;


/* ============================================================
   ROUTE TABLE
   ============================================================
   Stores route information associated with an event.
   Each route belongs to one event.
   The route can contain information such as the route name,
   description and route details.
   ============================================================ */

CREATE TABLE ROUTE
(
    ROUTE_ID INT IDENTITY(1,1) PRIMARY KEY,
    EVENT_ID INT NOT NULL,
    ROUTE_NAME VARCHAR(100) NOT NULL,
    ROUTE_DESCRIPTION VARCHAR(500),
    ROUTE_LOCATION VARCHAR(255),

    -- Creates the relationship between ROUTE and EVENT.
    CONSTRAINT FK_ROUTE_EVENT
        FOREIGN KEY (EVENT_ID)
        REFERENCES EVENT(EVENT_ID)
);

SELECT * FROM ROUTE;


/* ============================================================
   1. ALTER EVENT TABLE
   ============================================================

   Rename the existing EVENT attributes so that their names
   exactly match the attributes shown in the ERD.
   ============================================================ */

-- Rename DESCRIPTION to EVENT_DESCRIPTION.
EXEC sp_rename
    'EVENT.DESCRIPTION',
    'EVENT_DESCRIPTION',
    'COLUMN';

    SELECT * FROM EVENT;

-- Rename LOCATION to EVENT_LOCATION.
EXEC sp_rename
    'EVENT.LOCATION',
    'EVENT_LOCATION',
    'COLUMN';

-- Rename DISTANCE to EVENT_DISTANCE.
EXEC sp_rename
    'EVENT.DISTANCE',
    'EVENT_DISTANCE',
    'COLUMN';

    /* ============================================================ 
    ADD BANNER_IMAGE_URL TO EVENT 
    ============================================================ 
    
    BANNER_IMAGE_URL stores the URL of the event banner image. 
    The actual image will later be stored using Azure Blob Storage in Part 3. 
    The database only needs to store the URL that allows the application to retrieve/display it. 
    NULL is allowed because an event may initially be created without a banner image. 
    */ 
    ALTER TABLE EVENT 
    ADD BANNER_IMAGE_URL VARCHAR(500) NULL; 

    /* ============================================================ 
    2. ALTER CATEGORY TABLE 
    ============================================================ 
    Rename DESCRIPTION to CATEGORY_DESCRIPTION so that the attribute name matches the final ERD. 
    ============================================================ */ 
    EXEC sp_rename 
    'CATEGORY.DESCRIPTION', 
    'CATEGORY_DESCRIPTION', 
    'COLUMN';

    SELECT * FROM CATEGORY;

    /* ============================================================ 
    3. ALTER ROUTE TABLE 
    ============================================================ 
    Add ROUTE_URL to store the URL/reference for the route. 
    This allows the RaceDay system to associate an event with an online route resource. 
    ============================================================ */ 
    ALTER TABLE ROUTE 
    ADD ROUTE_URL VARCHAR(500) NULL;

    SELECT* FROM ROUTE;

    SELECT * FROM [USER];
    SELECT * FROM EVENT;
    SELECT * FROM CATEGORY;
    SELECT * FROM RESULT;
    SELECT * FROM ENROLMENT;
    SELECT * FROM ROUTE;

    /* ============================================================ 
    SAMPLE DATA / DATABASE SEEDING 
    ============================================================ 
    The following INSERT statements provide realistic sample data for testing the RaceDay system.
    Minimum requirements satisfied: - 2 Organisers - 2 Participants - 3 Events 
    - Categories for each event - Sample enrolments -
    ============================================================ */ 
    /* ============================================================ 
    1. SAMPLE USERS 
    ============================================================ 
    Two Organisers and two Participants are created. 
    The ROLE attribute determines whether the user is an Organiser or Participant. 
    PASSWORD_HASH stores a hashed password rather than the user's original password. 
    ============================================================ */ 
    INSERT INTO [USER] (FIRST_NAME, LAST_NAME, EMAIL,PHONE_NUMBER, PASSWORD_HASH, ROLE) 
    VALUES  ('Realeboga', 'Moche', 'rea.moche@gmail.com',0677046663, 'HASHED_PASSWORD_001', 'Organiser'), 
            ('Wanga', 'Tshidada', 'wanga.tshidada@icloud.com',0712345678, 'HASHED_PASSWORD_002', 'Organiser'), 
            ('Thato', 'Kekana', 'thato.kekana@gmail.com',0667048778, 'HASHED_PASSWORD_003', 'Participant'), 
            ('Bonolo', 'Toka', 'bonolo.toka@icloud.com',NULL, 'HASHED_PASSWORD_004', 'Participant'); 

    SELECT * FROM [USER];

    /* ============================================================ 
    2. SAMPLE EVENTS 
    ============================================================ 
    Three events are created. Each event contains the required information: 
    - Name - Description - Date - Location - Distance - Event type, therefore every event must contain a value. 
    ============================================================ */ 
    INSERT INTO EVENT ( ORGANISER_ID, EVENT_NAME, EVENT_DESCRIPTION, EVENT_DATE, EVENT_LOCATION, EVENT_DISTANCE, EVENT_TYPE ) 
    VALUES  ( 5, 'Pretoria City Run', 'A community road running event through Pretoria.', '2026-10-10', 'Pretoria', 10.00, 'Run'), 
            ( 6, 'Johannesburg Charity Walk', 'A charity walking event supporting local communities.', '2026-11-14', 'Johannesburg', 5.00, 'Walk' ), 
            ( 5, 'Gauteng Cycle Challenge', 'A cycling challenge for recreational and competitive cyclists.', '2026-12-05', 'Centurion', 50.00, 'Cycle' ); 

    SELECT * FROM EVENT;
    

