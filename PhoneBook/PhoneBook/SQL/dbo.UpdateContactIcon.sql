CREATE PROCEDURE [dbo].[UpdateContactIcon]
	@id INT,
	@i IMAGE
AS
	UPDATE Contact
	SET Icon = @i
	WHERE Id = @id
RETURN 0
