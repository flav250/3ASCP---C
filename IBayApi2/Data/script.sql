CREATE TABLE Member (
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
                          FOREIGN KEY (UserId) REFERENCES Member(Id)
);

CREATE TABLE Cart (
                       Id INT PRIMARY KEY IDENTITY(1,1),
                       UserId INT,
                       Buy BIT,
                       FOREIGN KEY (UserId) REFERENCES Member(Id)
);

CREATE TABLE Cart_Item (
                               Id INT PRIMARY KEY IDENTITY(1,1),
                               CartId INT,
                               ProductId INT,
                               Quantity INT,
                               FOREIGN KEY (CartId) REFERENCES Cart(Id),
                               FOREIGN KEY (ProductId) REFERENCES Product(ID)
);