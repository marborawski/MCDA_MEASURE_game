clear all

file_code = 'VIKOR_wynik-2010-2016';
sheet_code = '2015';
column_code_in_file_code = 1;
column_code = 1;
column_measure = 3;
resultFile = 'result';


t = 1;
files{t} = 'VIKOR_wynik-2010-2016.xls';t = t + 1;
files{t} = 'VIKOR_wynik-2005-2016.xls';t = t + 1;
files{t} = 'VIKOR_wynik-2005-2009.xls';t = t + 1;

t = 1;
lata{t} = '2005';t = t + 1;
lata{t} = '2006';t = t + 1;
lata{t} = '2007';t = t + 1;
lata{t} = '2008';t = t + 1;
lata{t} = '2009';t = t + 1;
lata{t} = '2010';t = t + 1;
lata{t} = '2011';t = t + 1;
lata{t} = '2012';t = t + 1;
lata{t} = '2013';t = t + 1;
lata{t} = '2014';t = t + 1;
lata{t} = '2015';t = t + 1;
lata{t} = '2016';t = t + 1;

info = cell(1,length(files));
for ii = 1:length(info)
    [a,info{ii}] = xlsfinfo(files{ii});    
end;

result = cell(1,length(info));
style = cell(1,length(info));
color = cell(1,length(info));

[a b d] = xlsread(file_code,sheet_code);
sheets = cell(1,length(info));
for k=1:length(result)
    sheets{k} = num2str(k);
    result{k}{1,1} = files{k};
    style{k}{1,1} = 's';
    color{k}{1,1} = [];
    for ii = 1:length(lata)
        result{k}{2,ii + 2} = lata{ii};
        style{k}{2,ii} = 's';
        color{k}{2,ii} = [];
    end
end

for ii = 1:size(d,1)
    for jj = 1:2
        for k=1:length(result)
            result{k}{ii + 1,jj} = d{ii,jj};
            style{k}{ii + 1,jj} = 's';
            color{k}{ii + 1,jj} = [];
        end
    end
end

for ii = 1:length(lata)
    for jj = 1:length(info)
        no = 0;
        for k = 1:length(info{jj})
            if strcmp(info{jj}(k),lata{ii})
                no = k;
                break;
            end
        end
        if no > 0
            [a b data] = xlsread(files{jj},info{jj}{no});
            for iii = 2:size(d,1)
                for jjj = 2:size(data,1)
                    if strcmp(d{iii,column_code_in_file_code},data{jjj,column_code})
                        result{jj}{iii + 1,ii + 2} = data{jjj,column_measure};
                        style{jj}{iii + 1,ii + 2} = 'd';
                        color{jj}{iii + 1,ii + 2} = [];
                        break;
                    end
                end
            end
        end
    end;

    
end;

saveXLS(resultFile,sheets,result,style,color);


