-- Script Date: 13.11.2019 16:57  - ErikEJ.SqlCeScripting version 3.5.2.84
SELECT 1;
PRAGMA foreign_keys=ON;
BEGIN TRANSACTION;
CREATE TABLE [Profile] (
  [Id] uniqueidentifier NOT NULL
, [MainName] nvarchar(300) NOT NULL COLLATE NOCASE
, [Loisy] int NOT NULL
, [Zashkvory] int NOT NULL
, [Slivi] int DEFAULT (0) NOT NULL
, [CanVote] bit DEFAULT (1) NOT NULL
, [IsTransport] bit DEFAULT (0) NOT NULL
, CONSTRAINT [PK_Profile] PRIMARY KEY ([Id])
);
CREATE TABLE [ProfileName] (
  [ProfileId] uniqueidentifier NOT NULL
, [Name] nvarchar(300) NOT NULL COLLATE NOCASE
, CONSTRAINT [PK_ProfileName] PRIMARY KEY ([ProfileId],[Name])
, CONSTRAINT [FK_ProfileName_Profile] FOREIGN KEY ([ProfileId]) REFERENCES [Profile] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [ProfileJid] (
  [ProfileId] uniqueidentifier NOT NULL
, [Jid] nvarchar(300) NOT NULL COLLATE NOCASE
, CONSTRAINT [PK_ProfileJid] PRIMARY KEY ([ProfileId],[Jid])
, CONSTRAINT [FK_ProfileJid_Profile] FOREIGN KEY ([ProfileId]) REFERENCES [Profile] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE [Note] (
  [NoteName] nvarchar(140) NOT NULL
, [Content] ntext NOT NULL
, [Added] datetime NOT NULL
, [DisplayNoteName] nvarchar(200) DEFAULT ('s') NOT NULL COLLATE NOCASE
, [Updated] datetime NOT NULL
, CONSTRAINT [PK_Note] PRIMARY KEY ([NoteName])
);
CREATE TABLE [Attribute] (
  [Attribute] nvarchar(200) NOT NULL
, [Id] uniqueidentifier NOT NULL
, CONSTRAINT [PK_Attribute] PRIMARY KEY ([Id])
);
CREATE TABLE [ProfileAttribute] (
  [ProfileId] uniqueidentifier NOT NULL
, [AttributeId] uniqueidentifier NOT NULL
, [Value] ntext NOT NULL
, CONSTRAINT [PK_ProfileAttribute] PRIMARY KEY ([ProfileId],[AttributeId])
, CONSTRAINT [FK_ProfileAttribute_Attribute] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
, CONSTRAINT [FK_ProfileAttribute_Profile] FOREIGN KEY ([ProfileId]) REFERENCES [Profile] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
CREATE TRIGGER [fki_ProfileName_ProfileId_Profile_Id] BEFORE Insert ON [ProfileName] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table ProfileName violates foreign key constraint FK_ProfileName_Profile') WHERE (SELECT Id FROM Profile WHERE  Id = NEW.ProfileId) IS NULL; END;
CREATE TRIGGER [fku_ProfileName_ProfileId_Profile_Id] BEFORE Update ON [ProfileName] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table ProfileName violates foreign key constraint FK_ProfileName_Profile') WHERE (SELECT Id FROM Profile WHERE  Id = NEW.ProfileId) IS NULL; END;
CREATE TRIGGER [fki_ProfileJid_ProfileId_Profile_Id] BEFORE Insert ON [ProfileJid] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table ProfileJid violates foreign key constraint FK_ProfileJid_Profile') WHERE (SELECT Id FROM Profile WHERE  Id = NEW.ProfileId) IS NULL; END;
CREATE TRIGGER [fku_ProfileJid_ProfileId_Profile_Id] BEFORE Update ON [ProfileJid] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table ProfileJid violates foreign key constraint FK_ProfileJid_Profile') WHERE (SELECT Id FROM Profile WHERE  Id = NEW.ProfileId) IS NULL; END;
CREATE TRIGGER [fki_ProfileAttribute_AttributeId_Attribute_Id] BEFORE Insert ON [ProfileAttribute] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table ProfileAttribute violates foreign key constraint FK_ProfileAttribute_Attribute') WHERE (SELECT Id FROM Attribute WHERE  Id = NEW.AttributeId) IS NULL; END;
CREATE TRIGGER [fku_ProfileAttribute_AttributeId_Attribute_Id] BEFORE Update ON [ProfileAttribute] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table ProfileAttribute violates foreign key constraint FK_ProfileAttribute_Attribute') WHERE (SELECT Id FROM Attribute WHERE  Id = NEW.AttributeId) IS NULL; END;
CREATE TRIGGER [fki_ProfileAttribute_ProfileId_Profile_Id] BEFORE Insert ON [ProfileAttribute] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Insert on table ProfileAttribute violates foreign key constraint FK_ProfileAttribute_Profile') WHERE (SELECT Id FROM Profile WHERE  Id = NEW.ProfileId) IS NULL; END;
CREATE TRIGGER [fku_ProfileAttribute_ProfileId_Profile_Id] BEFORE Update ON [ProfileAttribute] FOR EACH ROW BEGIN SELECT RAISE(ROLLBACK, 'Update on table ProfileAttribute violates foreign key constraint FK_ProfileAttribute_Profile') WHERE (SELECT Id FROM Profile WHERE  Id = NEW.ProfileId) IS NULL; END;
COMMIT;

