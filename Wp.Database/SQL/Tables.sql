DROP TABLE IF EXISTS Guild;
DROP TABLE IF EXISTS Clan;
DROP TABLE IF EXISTS Calendar;
DROP TABLE IF EXISTS Role;
DROP TABLE IF EXISTS Competition;
DROP TABLE IF EXISTS Time;
DROP TABLE IF EXISTS Player;
DROP TABLE IF EXISTS WarStatistic;
DROP TABLE IF EXISTS PlayerStatistic;

CREATE TABLE Guild (
    Id DECIMAL(25, 0) NOT NULL,
    Language INT NOT NULL,
    TimeZone INT NOT NULL,
    PremiumLevel INT NOT NULL,
	MinTHLevel INT NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE Clan (
    Guild DECIMAL(25, 0) NOT NULL,
    Tag VARCHAR(50) NOT NULL,
    PRIMARY KEY (Guild, Tag),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE Calendar (
    Guild DECIMAL(25, 0) NOT NULL,
    CalendarId VARCHAR(100) NOT NULL,
    ChannelId DECIMAL(25, 0),
    MessageId DECIMAL(25, 0),
    PRIMARY KEY (Guild, CalendarId),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE Role (
    Guild DECIMAL(25, 0) NOT NULL,
    Id DECIMAL(25, 0) NOT NULL,
    Type INT NOT NULL,
    PRIMARY KEY (Id, Type),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE Competition (
    Guild DECIMAL(25,0) NOT NULL,
    CategoryId DECIMAL(25, 0) NOT NULL,
    ResultId DECIMAL(25, 0) NOT NULL,
	Name VARCHAR(MAX) NOT NULL,
    MainClan VARCHAR(50) NOT NULL,
    SecondClan VARCHAR(50),
    PRIMARY KEY (Guild, CategoryId),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE Time (
    Guild DECIMAL(25, 0) NOT NULL,
    Action INT NOT NULL,
    Date DATETIME NOT NULL,
    Interval INT NOT NULL,
    Additional VARCHAR(100) NOT NULL,
    Optional VARCHAR(MAX),
    PRIMARY KEY (Guild, Action, Additional),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE Player (
    Guild DECIMAL(25, 0) NOT NULL,
    DiscordId DECIMAL(25, 0) NOT NULL,
    Tag VARCHAR(50) NOT NULL,
    PRIMARY KEY (Guild, Tag),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE WarStatistic (
    Guild DECIMAL(25, 0) NOT NULL,
    DateStart DATETIME NOT NULL,
    WarType INT NOT NULL,
    ClanTag VARCHAR(50) NOT NULL,
    CompetitionCategory DECIMAL(25, 0),
    OpponentName VARCHAR(50) NOT NULL,
    Result INT NOT NULL,
    AttackStars INT NOT NULL,
    AttackPercent DECIMAL(5, 2) NOT NULL,
    AttackAvgDuration DECIMAL(5, 2) NOT NULL,
    DefenseStars INT NOT NULL,
    DefensePercent DECIMAL(5, 2) NOT NULL,
    DefenseAvgDuration DECIMAL(5, 2) NOT NULL,
    PRIMARY KEY (DateStart, ClanTag),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE PlayerStatistic (
    Guild DECIMAL(25, 0) NOT NULL,
    DiscordId DECIMAL(25, 0) NOT NULL,
    PlayerTag VARCHAR(50) NOT NULL,
    ClanTag VARCHAR(50) NOT NULL,
    WarDateStart DATETIME NOT NULL,
    AttackOrder INT NOT NULL,
    WarType INT NOT NULL,
    StatisticType INT NOT NULL,
    StatisticAction INT NOT NULL,
    Stars INT NOT NULL,
    Percentage INT NOT NULL,
    Duration INT NOT NULL,
    PRIMARY KEY (DiscordId, ClanTag, WarDateStart, AttackOrder),
    FOREIGN KEY (Guild)
        REFERENCES Guild(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
