clear all
close all

t = tcpclient('localhost', 55001); 
writeline(t,"hello\n")
readline(t,1,"string")


clear t;

return
t = tcpip('localhost', 55001); 
% Set size of receiving buffer, if needed. 
%set(t, 'InputBufferSize', 30000); 
% Open connection to the server. 
fopen(t); 
% Transmit data to the server (or a request for data from the server). 
fprintf(t, 'GET /'); 
% Pause for the communication delay, if needed. 
pause(1) 
% Receive lines of data from server 
disp('a');
t
while (get(t, 'BytesAvailable') > 0) 
  disp('b');
t.BytesAvailable 
DataReceived = fscanf(t) 
end 
% Disconnect and clean up the server connection. 
fclose(t); 
%delete(t); 
clear t 