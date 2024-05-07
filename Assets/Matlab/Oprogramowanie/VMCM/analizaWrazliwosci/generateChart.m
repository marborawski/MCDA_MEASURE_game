clear all
%close all;

color = 'rgbcmyk';

file = 'result';

t = 1;
sheet{t} = '1';t = t + 1;
sheet{t} = '2';t = t + 1;
sheet{t} = '3';t = t + 1;

t = 1;
code{t} = 'PL';t = t + 1;
code{t} = 'BG';t = t + 1;
code{t} = 'CY';t = t + 1;
code{t} = 'HU';t = t + 1;
code{t} = 'LT';t = t + 1;
code{t} = 'LV';t = t + 1;
code{t} = 'RO';t = t + 1;

column_code = 2;

data = cell(1,length(sheet));
for ii = 1:length(sheet)
    [a,b,data{ii}] = xlsread(file,sheet{ii});    
end;


for ii = 1:length(code)
    for jj = 1:length(data)
        for k = 1:size(data{1},1)
            if strcmp(data{jj}{k,column_code},code{ii})
                plot(dataXLS2num(data{jj}(2,3:end)),dataXLS2num(data{jj}(k,3:end)),color(rem(ii - 1,length(color)) + 1));hold on;
                break;
            end
        end
    end
end

jj = 1;
for ii = 1:length(code)
    code2{jj} = code{ii};jj = jj + 1;
    code2{jj} = code{ii};jj = jj + 1;
    code2{jj} = code{ii};jj = jj + 1;
end

title('Topsis');
legend(code2);