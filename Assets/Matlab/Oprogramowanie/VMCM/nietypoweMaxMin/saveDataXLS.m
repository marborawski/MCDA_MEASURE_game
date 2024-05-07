function saveDataXLS( file,data,lata,zmienne,nazwy_wariantow_decyzyjnych,code,style2,color2 )
%UNTITLED Summary of this function goes here
%   Detailed explanation goes here
res = cell(1,length(data));
for ii = 1:length(data)
    res{ii} = cell(size(data{ii},1) + 1,size(data{ii},2) + 2);
    res{ii}{1,1} = 'Obiekty';
    res{ii}{1,2} = 'Kody';
    for jj = 1:length(zmienne)
        res{ii}{1,jj + 2} = zmienne{jj};
    end
    for jj = 1:length(nazwy_wariantow_decyzyjnych)
        res{ii}{jj + 1,1} = nazwy_wariantow_decyzyjnych{jj};
    end
    for jj = 1:length(code)
        res{ii}{jj + 1,2} = code{jj};
    end
    for jj = 1:size(data{ii},1)
        for k = 1:size(data{ii},2)
            if isnan(data{ii}(jj,k))
                res{ii}{jj + 1,k + 2} = '-';
            else
                res{ii}{jj + 1,k + 2} = data{ii}(jj,k);
            end
        end
    end
end



style = cell(1,length(res));
color = cell(1,length(res));
for ii = 1:length(res)
    style{ii} = cell(size(res{ii}));
    color{ii} = cell(size(res{ii}));
    for jj = 1:size(res{ii},1)
        for k = 1:size(res{ii},2)
            if jj > 1 && k > 2
                if isnan(data{ii}(jj-1,k-2))
                    style{ii}{jj,k} = 's';
                    color{ii}{jj,k} = 'red';
                else
                    style{ii}{jj,k} = 'd';
                    color{ii}{jj,k} = [];
                end
            else
                style{ii}{jj,k} = 's';
                color{ii}{jj,k} = [];
            end
        end
    end
end

if ~isempty(color2)
    color = color2;
end

if ~isempty(style2)
    style = style2;
end


saveXLS(file,lata,res,style,color);

end

