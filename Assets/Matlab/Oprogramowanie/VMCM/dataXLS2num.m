function res = dataXLS2num(data)
    res = zeros(size(data));
    for ii = 1:length(data)
        if isnumeric(data{ii})
            res(ii) = data{ii};
        else
            tmp = str2num(data{ii});
            if isempty(tmp)
                res(ii) = NaN;
            else
                res(ii) = tmp;
            end
        end
    end
end