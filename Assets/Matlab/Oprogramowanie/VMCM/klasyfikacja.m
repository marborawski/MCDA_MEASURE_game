function [ res ] = klasyfikacja( data,liczbaKlas )
%UNTITLED3 Summary of this function goes here
%   Detailed explanation goes here

    tmp = [0:1/liczbaKlas:1];

    res = zeros(size(data));
    
    q = quantile(data,tmp);
    for ii = 1:size(data,1)
        for jj = 2:length(q)
            if jj == 2
                if data(ii) <= q(jj)
                    res(ii) = jj - 1;
                    break;
                end
            else
                if data(ii) > q(jj-1) && data(ii) <= q(jj)
                    res(ii) = jj - 1;
                    break;
                end
            end
        end
    end
    res = liczbaKlas - res + 1;

end

