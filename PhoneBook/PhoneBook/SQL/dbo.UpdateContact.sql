CREATE PROCEDURE [dbo].[UpdateContact]
	@id INT,
	@fn VARCHAR(50),
	@ln VARCHAR(50),
	@pn VARCHAR(20),
	@ea VARCHAR(50),
	@c VARCHAR(50),
	@f BIT
AS
	UPDATE Contact
	SET FirstName=@fn, LastName=@ln, PhoneNumber=@pn, EmailAddress=@ea, Company=@c, Favourite=@f
	WHERE Id=@id
RETURN 0