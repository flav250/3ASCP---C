GO
USE master;
GO
CREATE DATABASE IBAyApi;
GO
USE IBAyApi;
GO

CREATE TABLE Member
(
    Id       INT PRIMARY KEY IDENTITY (1,1),
    Email    VARCHAR(250) NOT NULL,
    Pseudo   VARCHAR(250) NOT NULL,
    Password VARCHAR(250) NOT NULL,
    Role     VARCHAR(250)
);

CREATE TABLE Product
(
    Id        INT PRIMARY KEY IDENTITY (1,1),
    Name      VARCHAR(250) NOT NULL,
    Image     VARCHAR(250) NOT NULL,
    Price     FLOAT        NOT NULL,
    Available BIT          NOT NULL,
    AddedTime DATETIME     NOT NULL,
    UserId    INT,
    FOREIGN KEY (UserId) REFERENCES Member (Id)
);

CREATE TABLE Cart
(
    Id     INT PRIMARY KEY IDENTITY (1,1),
    UserId INT,
    Buy    BIT,
    FOREIGN KEY (UserId) REFERENCES Member (Id)
);

CREATE TABLE CartItem
(
    Id        INT PRIMARY KEY IDENTITY (1,1),
    CartId    INT,
    ProductId INT,
    Quantity  INT,
    FOREIGN KEY (CartId) REFERENCES Cart (Id),
    FOREIGN KEY (ProductId) REFERENCES Product (ID)
);


INSERT INTO Member (Email, Pseudo, Password, Role)
VALUES ('user1@gmail.com', 'user1', '0a041b9462caa4a31bac3567e0b6e6fd9100787db2ab433d96f6d178cabfce90', 'seller');
INSERT INTO Member (Email, Pseudo, Password, Role)
VALUES ('user2@gmail.com', 'user2', '6025d18fe48abd45168528f18a82e265dd98d421a7084aa09f61b341703901a3', 'seller');
INSERT INTO Member (Email, Pseudo, Password, Role)
VALUES ('user3@gmail.com', 'user3', '5860faf02b6bc6222ba5aca523560f0e364ccd8b67bee486fe8bf7c01d492ccb', 'buyer');

INSERT INTO Product (Name, Image, Price, Available, AddedTime, UserId)
VALUES ('product1', 'image1', 10, 1, '2021-01-01', 1);
INSERT INTO Product (Name, Image, Price, Available, AddedTime, UserId)
VALUES ('product2', 'image2', 5, 1, '2021-02-01', 1);
INSERT INTO Product (Name, Image, Price, Available, AddedTime, UserId)
VALUES ('product3', 'image3', 15, 1, '2021-03-01', 2);

INSERT INTO Cart (UserId, Buy)
VALUES (1, 0);
INSERT INTO Cart (UserId, Buy)
VALUES (2, 0);
INSERT INTO Cart (UserId, Buy)
VALUES (3, 0);

INSERT INTO CartItem (CartId, ProductId, Quantity)
VALUES (1, 1, 5);
INSERT INTO CartItem (CartId, ProductId, Quantity)
VALUES (1, 2, 10);
INSERT INTO CartItem (CartId, ProductId, Quantity)
VALUES (2, 2, 7);
INSERT INTO CartItem (CartId, ProductId, Quantity)
VALUES (3, 1, 3);
INSERT INTO CartItem (CartId, ProductId, Quantity)
VALUES (3, 2, 2);
INSERT INTO CartItem (CartId, ProductId, Quantity)
VALUES (3, 3, 8);