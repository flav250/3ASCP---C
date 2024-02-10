CREATE TABLE Users (
                       Id INT PRIMARY KEY IDENTITY(1,1),
                       Email VARCHAR(250) NOT NULL,
                       Pseudo VARCHAR(250) NOT NULL,
                       Password VARCHAR(250) NOT NULL,
                       Role VARCHAR(250)
);

CREATE TABLE Product (
                          Id INT PRIMARY KEY IDENTITY(1,1),
                          Name VARCHAR(250) NOT NULL,
                          Image VARCHAR(250) NOT NULL,
                          Price FLOAT NOT NULL,
                          Available BIT NOT NULL,
                          Added_time DATETIME NOT NULL,
                          UserId INT,
                          FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE Card (
                       Id INT PRIMARY KEY IDENTITY(1,1),
                       UserId INT,
                       Buy BIT,
                       FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE Card_Item (
                               Id INT PRIMARY KEY IDENTITY(1,1),
                               CardId INT,
                               ProductId INT,
                               Quantity INT,
                               FOREIGN KEY (CardId) REFERENCES Card(Id),
                               FOREIGN KEY (ProductId) REFERENCES Product(ID)
);