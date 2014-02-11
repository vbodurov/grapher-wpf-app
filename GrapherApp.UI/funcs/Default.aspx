<%@ Page language="c#" %>
<%@ Import namespace="System.IO" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title><%=Server.MapPath(".")%></title>
  </head>
  <body MS_POSITIONING="GridLayout">
	
    <%
    
    string strFile = Server.MapPath(@".")+"/"+Request["f"];
    if(strFile!=null && File.Exists(strFile)){
		/*FileStream fs = File.Open(strFile, FileMode.Open);
		
		try{
			Response.ClearContent();
			Response.ContentType = @"application/x-www-form-urlencoded";
			byte[] buffer = new byte[1024];
			int offset = 0;
			int count = 1024;
			offset = fs.Read( buffer, offset, count );
			Response.BinaryWrite( buffer );
			while(offset>0){	
				offset = fs.Read( buffer, offset, count );
				Response.BinaryWrite( buffer );
			}
			Response.Flush();
		}catch{}
		fs.Close();
		Response.End();*/
		Response.Redirect(Request["f"]);
    }
    
    
		string path = Server.MapPath(@".")+"/";
		DirectoryInfo di = new DirectoryInfo(path);
		FileInfo[] fileInfo = di.GetFiles();
		for(int i=0;i<fileInfo.Length;i++){
      var name = fileInfo[i].Name.ToLower();
			if(name =="default.aspx") continue;
			if(name.EndsWith(".png") || name.EndsWith(".jpg")){
          Response.Write( String.Format( "<img src='{0}'/>", fileInfo[i].Name ) );
          Response.Write( "<br/>" );
          continue;
			}
			Response.Write( String.Format( "<a href='Default.aspx?f={0}'>", fileInfo[i].Name ) );
			Response.Write( fileInfo[i].Name );
			Response.Write( "</a><br>" );
		}
		DirectoryInfo[] dirInfo = di.GetDirectories();
		for(int i=0;i<dirInfo.Length;i++){
			Response.Write( String.Format( "<a href='{0}'>",dirInfo[i].Name ) );
			Response.Write( "<font color=green>" );
			Response.Write( String.Concat("DIR ", dirInfo[i].Name ) );
			Response.Write( "</font>" );
			Response.Write( "</a><br><br>" );
		}
	
    
    
    %>
	
  </body>
</html>
