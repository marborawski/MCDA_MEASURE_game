function [data color] = interpolacja(data)

color = cell(1,length(data));
for k = 1:length(data)
    color{k} = cell(size(data{k},1)+1,size(data{k},1)+2);
end
for ii = 1:size(data{1},1)
    for jj = 1:size(data{1},2)
        d = zeros(1,length(data));
        for k = 1:length(data)
            d(k) = data{k}(ii,jj);
            color{k}{ii+1,jj+2} = [];
        end
        p = find(~isnan(d));
        if length(p) == 1
            for k = 1:length(data)
                data{k}(ii,jj) = data{p(1)}(ii,jj);
                if k ~= p(1)
                    color{ii}{jj,k} = 'red';
                end
            end
        else
            if length(p) < length(d)
                t = [1:length(d)];
                t = t(p);
%                SSY = sum((d(p)-mean(d(p))).^2);
                SSX = sum((t - mean(t)).^2);
                SSXY = sum((d(p) - mean(d(p))).*(t-mean(t)));
                a = SSXY/SSX;
                b = mean(d(p))-a*mean(t);
                for k = 1:length(data)
                    if isnan(data{k}(ii,jj))
                        data{k}(ii,jj) = a*k + b;
                        color{k}{ii+1,jj+2} = 'red';
                    end
                end
            end
        end
    end
end
