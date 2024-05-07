function [res nazwy_wariantow_decyzyjnych code] = readData(nazwa_pliku,lata)

res = cell(1,length(lata));
for ii = 1:length(lata)
    [a b dane] = xlsread(nazwa_pliku,lata{ii});
    res{ii} = zeros(size(dane,1)-1,size(dane,2)-2);
    for jj = 2:size(dane,1)
        for k = 3:size(dane,2)
            if isnumeric(dane{jj,k})
                res{ii}(jj-1,k - 2) = dane{jj,k};
            else
                res{ii}(jj-1,k - 2) = NaN;
            end
        end
    end
    if ii == 1
        for jj = 2:size(dane,1)
            nazwy_wariantow_decyzyjnych{jj - 1} = dane{jj,2};
            code{jj - 1} = dane{jj,1};
        end
    end
end
