clear all

nazwa_pliku = 'dane_do_obliczen_2016';
plik_wynikowy = 'WASPAS_wyniki_2016';

zmienna = 3;
panstwo = 10;
multiStd = 6;
stepStd = 0.25;

liczbaKlas = 4;

t = 1;

%  lata{t} = '2005';t = t + 1;
%  lata{t} = '2006';t = t + 1;
%  lata{t} = '2007';t = t + 1;
%  lata{t} = '2008';t = t + 1;
%  lata{t} = '2009';t = t + 1;
%  lata{t} = '2010';t = t + 1;
%  lata{t} = '2011';t = t + 1;
%  lata{t} = '2012';t = t + 1;
%  lata{t} = '2013';t = t + 1;
% lata{t} = '2014';t = t + 1;
%  lata{t} = '2015';t = t + 1;
lata{t} = '2016';t = t + 1;


% lambda=1;
% s_lambda = -0.15*lambda+0.3;

[a b param] = xlsread(nazwa_pliku,'parametry');

for ii = 2:size(param,1)
    zmienne{ii-1} = param{ii,1};
    wagi{ii-1} = param{ii,2};
    charakter{ii-1} = param{ii,3};
end

wagi = convert(wagi);

[data nazwy_wariantow_decyzyjnych code] = readData(nazwa_pliku,lata);

%saveDataXLS('Dane',data,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],[]);

[data color] = interpolacja(data);

%saveDataXLS('Interpolacja',data,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

wagi2 = wagi/sum(wagi);

tmp = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    tmp(ii) = sum(res);
end

weight3 = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    weight3(ii) = std(res)/mean(res);
end

weight3 = weight3/sum(weight3);

%wagi2 = weight3;

tmp = data{1}(:,zmienna);

mean_ = mean(tmp);
std_ = std(tmp);

nn = 0;
for iii = mean_ - multiStd*std_:stepStd*std_:mean_ + multiStd*std_
    nn = nn + 1;
end



tmp = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    if strcmp(charakter(ii),'s') == 1
        tmp(ii) = max(res);
    else
        tmp(ii) = min(res);
    end
end

dane_norm = cell(1,length(data));
for k = 1:length(data)
    dane_norm{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        if strcmp(charakter(ii),'s') == 1
            dane_norm{k}(:,ii) = data{k}(:,ii)/tmp(ii);
        else
            dane_norm{k}(:,ii) = tmp(ii)./data{k}(:,ii);
        end
    end
end

saveDataXLS('Normowanie',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);


Q1 = cell(1,length(dane_norm));
Q2 = cell(1,length(dane_norm));
Q_ = cell(1,length(dane_norm));
lambda = cell(1,length(dane_norm));
for k = 1:length(dane_norm)
    Q1{k} = zeros(size(dane_norm{1},1),1);
    Q2{k} = zeros(size(dane_norm{1},1),1);
    Q_{k} = zeros(size(dane_norm{1},1),1);
    for ii = 1:size(dane_norm{1},1)
        Q1{k}(ii) = sum(dane_norm{k}(ii,:).*wagi2);
        il = 1;
        for kk = 1:length(dane_norm{k}(ii,:))
            il = il*(dane_norm{k}(ii,kk)^wagi2(kk));
        end
        Q2{k}(ii) = il;
        lambda{k} = std(Q2{k})/(std(Q1{k}) + std(Q2{k}));
    end
    Q_{k}(:,1) = lambda{k}*Q1{k} + (1 - lambda{k})*Q2{k};
    
    Q_{k}(:,2) = klasyfikacja(Q_{k}(:,1),liczbaKlas);
end



Q2_{1} = [Q_{1}(1:panstwo-1,:);Q_{1}(panstwo+1:end,:)];

[a,b] = sort(Q2_{1}(:,1),'descend');

for ii = 1:panstwo-1
    code2{ii} = code{ii};
    nazwy_wariantow_decyzyjnych2{ii} = nazwy_wariantow_decyzyjnych{ii};
end
for ii = panstwo + 1:length(nazwy_wariantow_decyzyjnych)
    nazwy_wariantow_decyzyjnych2{ii - 1} = nazwy_wariantow_decyzyjnych{ii};
    code2{ii-1} = code{ii};
end

for ii = 1:length(nazwy_wariantow_decyzyjnych2)
    nazwy_wariantow_decyzyjnych3{ii} = nazwy_wariantow_decyzyjnych2{b(ii)};
    code3{ii} = code2{b(ii)};
end

Q2_{1}(:,1) = Q2_{1}(b,1);
%S2_{1}(:,2) = 0;
[a,b_] = sort(Q2_{1}(:,1),'descend');



start = 0;
iiii = 0;

for iii = mean_ - multiStd*std_:stepStd*std_:mean_ + multiStd*std_
    data{1}(panstwo,zmienna) = iii;
           
    tmp = zeros(1,size(data{1},2));
    for ii = 1:size(data{1},2)
        res = [];
        for k = 1:length(data)
            res = [res; data{k}(:,ii)];
        end
        if strcmp(charakter(ii),'s') == 1
            tmp(ii) = max(res);
        else
            tmp(ii) = min(res);
        end
    end

    dane_norm = cell(1,length(data));
    for k = 1:length(data)
        dane_norm{k} = zeros(size(data{k}));
        for ii = 1:size(data{1},2)
            if strcmp(charakter(ii),'s') == 1
                dane_norm{k}(:,ii) = data{k}(:,ii)/tmp(ii);
            else
                dane_norm{k}(:,ii) = tmp(ii)./data{k}(:,ii);
            end
        end
    end

    saveDataXLS('Normowanie',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);


    Q1 = cell(1,length(dane_norm));
    Q2 = cell(1,length(dane_norm));    
    lambda = cell(1,length(dane_norm));
    if start == 0
        Q = cell(1,length(dane_norm));
    end
    for k = 1:length(dane_norm)
        Q1{k} = zeros(size(dane_norm{1},1),1);
        Q2{k} = zeros(size(dane_norm{1},1),1);
        if start == 0
            Q{k} = zeros(size(dane_norm{1},1),2*nn);
        end
        for ii = 1:size(dane_norm{1},1)
            Q1{k}(ii) = sum(dane_norm{k}(ii,:).*wagi2);
            il = 1;
            for kk = 1:length(dane_norm{k}(ii,:))
                il = il*(dane_norm{k}(ii,kk)^wagi2(kk));
            end
            Q2{k}(ii) = il;
            lambda{k} = std(Q2{k})/(std(Q1{k}) + std(Q2{k}));
        end
        Q{k}(:,1 + iiii*2) = lambda{k}*Q1{k} + (1 - lambda{k})*Q2{k};

        Q{k}(:,2 + iiii*2) = klasyfikacja(Q{k}(:,1 + iiii*2),liczbaKlas);
    end

   
    start = 1;
    iiii = iiii + 1;
end

tmp = cell(1,1);
tmp{1} = 'Wartosc miary';
tmp{2} = 'Klasa';


saveDataXLS(plik_wynikowy,Q,lata,tmp,nazwy_wariantow_decyzyjnych,code,[],[]);

clear Q2;

Q2{1} = [Q{1}(1:panstwo-1,:);Q{1}(panstwo+1:end,:)];

% S2{1}(:,1) = S2{1}(b,1);
% S2{1}(:,2) = 0;
% [a,b_] = sort(S2{1}(:,1),'descend');

for ii = 1:2:size(Q2{1},2)
    Q2{1}(:,ii) = Q2{1}(b,ii);
    [a,b2] = sort(Q2{1}(:,ii),'descend');
    Q2{1}(:,ii + 1) = abs(b2 - b_);
end

saveDataXLS('wynik',Q2,lata,tmp,nazwy_wariantow_decyzyjnych3,code3,[],[]);


clear tmp;
s = mean_ - multiStd*std_;
s2 = -multiStd;

jj = 1;
for ii = 1:2:size(Q2{1},2)
    tmp{1,jj} = s; 
    tmp{2,jj} = s2; 
    tmp{3,jj} = sum(Q2{1}(:,ii + 1));
    tmp{4,jj} = sum(abs(Q2{1}(:,ii) - Q2_{1}(:,1))); 
    s = s + stepStd*std_;
    s2 = s2 + stepStd;
    jj = jj + 1;
end

xlswrite('wynik2.xls',tmp);
