function saveXLS(file,sheetName,data,style,color)

%fid = fopen([file '.xls'],'w','n','UTF-8');
fid = fopen([file '.xls'],'w');

fprintf(fid,'<?xml version="1.0"?>\n');
fprintf(fid,'<?mso-application progid="Excel.Sheet"?>\n');
fprintf(fid,'<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">\n');
fprintf(fid,'  <Styles>\n');
fprintf(fid,'    <Style ss:ID="s21">\n');
fprintf(fid,'    <Interior ss:Color="#FF0000" ss:Pattern="Solid"/>\n');
fprintf(fid,'    <NumberFormat/>\n');
fprintf(fid,'    </Style>\n');
fprintf(fid,'  </Styles>\n');
for kk = 1:length(sheetName)
    fprintf(fid,'    <Worksheet ss:Name="%s">\n',sheetName{kk});
    fprintf(fid,'      <Table>\n');

    [n m] = size(data{kk});
    [ns ms] = size(style{kk});
    [nc mc] = size(color{kk});


    for ii = 1:n
        fprintf(fid,'        <Row>\n');
        for jj = 1:m
            if ii <= nc && jj <= mc && ~isempty(color{kk}{ii,jj})
                fprintf(fid,'          <Cell ss:StyleID="s21"><Data ss:Type=');
            else
                fprintf(fid,'          <Cell><Data ss:Type=');
            end;
            if ii <= ns && jj <= ms && (strcmp(style{kk}{ii,jj},'n') || strcmp(style{kk}{ii,jj},'d')) 
                fprintf(fid,'"Number"');
            else
                fprintf(fid,'"String"');
            end
            fprintf(fid,'>'); 
            if ~isnan(data{kk}{ii,jj})
                if ii <= ns && jj <= ms && (strcmp(style{kk}{ii,jj},'n') || strcmp(style{kk}{ii,jj},'d')) 
                    fprintf(fid,'%d',data{kk}{ii,jj});
                else
                    fprintf(fid,'%s',data{kk}{ii,jj});
                end
            end
            fprintf(fid,'</Data></Cell>\n');
        end
        fprintf(fid,'        </Row>\n');
    end

    fprintf(fid,'      </Table>\n');
    fprintf(fid,'    </Worksheet>\n');
end
fprintf(fid,'</Workbook>');

fclose(fid);