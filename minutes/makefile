OBJ = $?	#all dependencies that are newer than target

all: *.tex
	for i in $(OBJ);  do \
           pdflatex $${i};   \
        done;	
	rm *.aux *.log;

clean:
	rm *.aux *.log
