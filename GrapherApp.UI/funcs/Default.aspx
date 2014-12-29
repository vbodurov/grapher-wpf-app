<%@ Page language="c#" %>
<%@ Import namespace="System.IO" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>funcs</title>
    <style>
    .tile{
      margin:20px;
      float:left;
      width:590px;
      font-family:sans-serif;
    }
    .tile h4{
      margin:3px;
    }
    .tile textarea{
      width:580px;
      height:30px;
    }
    </style>
  </head>
  <body>
	
    <%
    
    var dict = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase){
      {"f01", "pow(sin(x*PI), sin(x*PI))"},
      {"f02", "pow(sin(x*PI), atan(x*PI))"},
      {"f03", "pow(sin(x*PI), 3)"},
      {"f04", "pow(sin(x*PI), 4)"},
      {"f05", @"NORMALIZED:
var sd = 0.2f;
var avg = 0.5f;
return Math.Pow(Math.E, -0.5 * Math.Pow((x - avg)/sd, 2));

ORIGINAL:

var sd = 0.4f;
var avg = 0f;
return (
	1f / (Math.Sqrt(2 * Math.PI)*sd)) * 
	Math.Pow(Math.E, -0.5 * Math.Pow((x - avg)/sd, 2)
);"},
      {"f06", @"pow(abs(x*e), e) * pow(e, -abs(x*e))
-pow(abs(x*e), e) * pow(e, -abs(x*e)) + 1"},
      {"f07", "pow(sin(x*PI), 1/sin(x*PI))"},
      {"f08", @"(pow(x, 0.3) + (1 - pow(1 - x, 1/0.3))) / 2
(pow(x, COEF) + (1 - pow(1 - x, 1/COEF))) / 2
COEF => 0 - 1

(pow(x, 3.0) + (1 - pow(1 - x, 1/3.0))) / 2
(pow(x, COEF) + (1 - pow(1 - x, 1/COEF))) / 2
COEF => 1 - MAX
"},
      {"f09", @"sin(x * PI * 2) / (2 + x) + 0.5
-sin(x * PI * (1/x)*0.5) / (2 + x) + 0.5
sin(x * PI * (1/x)*0.5) / (2 + x) + 0.5"},
      {"f10", @"pow(abs(x), 0.05/pow(abs(x),2)) * sign(x)
pow(abs(x), CURVETURE/pow(abs(x),HOW_WIDE_IS_THE_BASE)) * sign(x)"},
      {"f11", @"var d = 4.5;
pow(abs(x)*e*d, e) * pow(e, -abs(x)*e*d)
pow(abs(x), pow(e, pow(abs(x), 2)))"},
      {"f12", @"var coef = 3.0;
-pow(2, coef) * pow(abs(x - 0.5), coef) + 1"},
      {"f13", @"elastic in out:
(71.25 * (x * (x * x)) * (x * x) + -176.5 * (x * x) * (x * x) + 145.5 * (x * (x * x)) + -43 * (x * x) + 3.75 * x);

elastic in:
(33 * (x * (x * x)) * (x * x) + -59*(x * x) * (x * x) + 32 * (x * (x * x)) + -5 * (x * x))

elastic out
(33 * (x * (x * x)) * (x * x) + -106*(x * x) * (x * x) + 126 * (x * (x * x)) + -67 * (x * x) + 15 * x)"},
      {"f14", "pow(5, -(pow((x - 0.842351), 2) / pow((2 * 0.234197), 2))) * 1.2"},
      {"f15", @"sign(x)*-pow(abs(x), 2.9)
sign(x)*-pow(abs(x), 1/2.9)
-x"},
      {"f16", @"sign(x)*pow(abs(x), 2.9)
sign(x)*pow(abs(x), 1/2.9)
x"},
      {"f17", @"(sin(pow(abs(x), 1.5) * PI - PI / 2) * 0.5f + 0.5f) * sign(x)"},
      {"f18", @"return max(
pow(abs(x-1)*e*5, e) * pow(e, -abs(x-1)*e*5), 
pow(sin(x*PI-PI/2),51)
);"},
      {"f19", @"pow(sin(   pow(x, 2)   *PI), 4);
pow(sin(x*PI), 4);
pow(sin(   (-pow(abs(x-1), 2)+1)    *PI), 4);
bell like shifted:

var shift = -1.5;
var power = 2;
return pow(abs(sin((shift > 1 ? pow(x, shift) : shift < -1 ? (-pow(abs(x-1), shift*-1)+1) : x) * PI)), power);

where power = [1 to MAX]
      shift = [-MAX to -1] for left shift OR [1 to MAX] for right shift

      "},
      {"f20", @"var curv = 1.2f;
return pow(abs(x-0.5), curv) * pow(2, curv);"},
      {"f21", @"sqrt(1 - x*x)
      
-sqrt(1 - x*x)

-----------------------------

var radius = 0.2f;
var centerY = 0.5f;
var centerX = 0.5f;
return sqrt(radius*radius - (x-centerX )*(x-centerX ))+centerY;

-----------------------------
var radius = 0.2f;
var centerY = 0.5f;
var centerX = 0.5f;
return -sqrt(radius*radius - (x-centerX )*(x-centerX ))+centerY;"}, 
      {"f22", @"sin(x*PI)*sign(x)
sin((x + 0.2*sign(x))*PI)*sign(x)
sin((x - 0.2*sign(x))*PI)*sign(x)"},
      {"f23", @"sin(x * PI*16 )*sqrt(1 - x*x)
sqrt(1 - x*x)"},
      {"f24", @"var sd = 0.2f;
var avg = 0.5f;
return Math.Pow(Math.E, -0.5 * Math.Pow((x - avg)/sd, 2))*sin(x * PI*16 );

var sd = 0.2f;
var avg = 0.5f;
return Math.Pow(Math.E, -0.5 * Math.Pow((x - avg)/sd, 2));"},
      {"f25", @"abs(sin(x*PI)*sin(x * PI*8 ))
abs(sin(x*PI))"},
      {"f26", @"(sin(x*PI)*0.5+0.5) * (sin(x * PI*8 )*0.5 + 0.5)
abs(sin(x*PI)*0.5 + 0.5)

(sin(x*PI*2-PI*0.5)*0.5+0.5) * (sin(x * PI*8 )*0.5 + 0.5)
abs(sin(x*PI*2-PI*0.5)*0.5 + 0.5)"},
      {"f27", @"max(sign(x-1), sqrt(max(1 - pow(x-1, 2.0), 0)))
-----------
max(max(sign(x-1), 0), 1-pow(x-1, 2))
-----------  
-cos(x*PI)*0.5f+0.5f
-----------
var sd = 0.2f;
var avg = 1f;
return round(pow(E, -0.5 * pow((x - avg)/sd, 2))*10000f)/10000f;
"},
      {"f28", @"var flatness = 2;
var period = x * PI;
return sqrt(
	(1 + flatness * flatness)
 	/
 	(1 + flatness * flatness * cos(period)*cos(period))
) * cos(period)
"}
    };
    
    var img = 0;
		string path = Server.MapPath(@".")+"/";
		DirectoryInfo di = new DirectoryInfo(path);
		FileInfo[] fileInfo = di.GetFiles();
		
		var cells = new List<string>();
		
		for(var i=0;i<fileInfo.Length;i++){
		

		
		
      var name = fileInfo[i].Name.ToLower();
			if(name =="default.aspx") continue;
			if(name.EndsWith(".png") || name.EndsWith(".jpg")){
          
          var key = name.Replace(".png", "").Replace(".jpg", "");
        
          string funcText = "";
          string s;
          if(dict.TryGetValue(key, out s)){
            funcText = s.Replace("<","&lt;").Replace(">","&gt;");
          }
        
          ++img;
          cells.Add( String.Format( "<div class='tile'><a name='graph-"+img+"'></a>"+
          "<h4>graph "+img+"</h4><img src='{0}'/><textarea>"+funcText+"</textarea></div>", fileInfo[i].Name ) );
          continue;
			}
		}
		
		var table = new StringBuilder();
		
		table.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
		for(var i=0;i < cells.Count;i+=2){
      table.Append("<tr>");
      table.Append("<td valign=\"top\">"+cells[i]+"</td>");
      table.Append("<td valign=\"top\">"+(cells.Count > (i+1) ? cells[i+1] : "")+"</td>");
      table.Append("</tr>");
		}
		table.Append("</table>");
		
		Response.Write( table );

    
    
    %>
    
	
  </body>
</html>