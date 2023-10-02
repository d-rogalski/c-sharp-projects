CREATE PROCEDURE [dbo].[GetContact]
	@id int
AS
	SELECT * FROM Contact WHERE Id = @id
RETURN 0
