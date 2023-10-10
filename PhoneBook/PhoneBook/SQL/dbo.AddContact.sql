CREATE PROCEDURE [dbo].[AddContact]
	@fn VARCHAR(50),
	@ln VARCHAR(50),
	@pn VARCHAR(20),
	@ea VARCHAR(50),
	@c VARCHAR(50),
	@i IMAGE,
	@f bit
AS
	DECLARE @id INT

	INSERT INTO Contact (FirstName, LastName, PhoneNumber, EmailAddress, Company, Icon, Favourite)
	VALUES (@fn, @ln, @pn, @ea, @c, @i, @f)

	SELECT @id = MAX(Id) FROM Contact
RETURN @id