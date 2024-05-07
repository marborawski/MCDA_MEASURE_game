function result = convert(data)

result = zeros(1,length(data));
for ii = 1:length(data)
    result(ii) = data{ii}(1);
end