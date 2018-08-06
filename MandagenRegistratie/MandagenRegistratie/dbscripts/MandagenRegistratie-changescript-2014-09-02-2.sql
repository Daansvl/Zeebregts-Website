UPDATE 
dbo.Project
SET
ProjectNr = (SELECT project_NR FROM Zeebregtsdb.dbo.MDRProject WHERE dbo.Project.ProjectIdOrigineel = Zeebregtsdb.dbo.MDRProject.project_ID)
