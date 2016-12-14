	.model tiny
	.code
	.386
	org 100h
start:
vvod1:					
	xor eax,eax			
	xor edx,edx			
	call clrscr
	mov ah,9			
	mov dx,offset message1			
	int 21h				
	mov ah,08h					
	int 21h				
	cmp al,'+'			 
	je cloj				
	cmp al,'-'			
	je vich				
	cmp al,'/'
	je dele				
	cmp al,'*'
	je rr1								
	jmp rr				
rr1:	jmp umnoj			
rr:	cmp al,'q'			
	je quit
	cmp al,'Q'
	je quit
	jmp short vvod1				
quit:	
	ret				
cloj:					
	call clrscr			
		
	call aa1			
	push dx				
	call bb1				
	pop dx							
	add ax,dx			
		
	call print
	
	call endstrd
	mov ah,9			
	mov dx,offset message2			
	int 21h
	mov ah,08h
	int 21h				
	jmp vvod1
vich:					
	
	call clrscr			
	call aa1				
	push dx
	call bb1					
	
	pop dx
	sub dx,ax			
	mov ax,dx			
	
	call print			
	
	call endstrd
	mov ah,9			
	mov dx,offset message2			
	int 21h
	mov ah,08h
	int 21h				
	jmp vvod1	

dele:					
	call clrscr			
	call aa1				
	push dx
	call bb1					
	
	pop dx
	xchg dx,ax
	mov cx,dx			
	xor dx,dx			
	div cx				

	call print
	
	call endstrd
	mov ah,9			 
	mov dx,offset message2			
	int 21h
	mov ah,08h
	int 21h				
	

	jmp vvod1
umnoj:					
	call clrscr			
	call aa1				
	push dx
	call bb1					
	pop dx
	
	mul edx				

	call print
	
	call endstrd
	mov ah,9			 
	mov dx,offset message2			
	int 21h
	mov ah,08h
	int 21h				
	jmp vvod1
clrscr:
	xor dx,dx			
	mov ah,02h			
	int 10h				
	mov bl,0000111b			
	mov cx,25*80				
	mov ax,0920h			
	int 10h				
	ret
	endp
aa1:					
	mov dx,offset aaaa
	mov ah,9	
	int 21h
	call mover
	mov dx,ax			
	ret
	endp
bb1:					
	mov dx,offset bbbb
	mov ah,9	
	int 21h
	call mover
	ret
	endp

mover:	
	mov dx,offset bufer		
	mov ah,0ah				
	int 21h				
	mov dx,offset endstr		
	mov ah,9				
	int 21h					

	xor di,di			
	xor ax,ax			
	mov cl,blength
	xor ch,ch
	xor bx,bx
	mov si,cx				
	mov cl,10			
tohex:
	mov bl,byte ptr bconteg[di]
	sub bl,'0'			
	jb  tata			
	cmp bl,9			
	ja  tata			
	mul cx				
	add ax,bx				
	inc di				
	cmp di,si			  	
	jb tohex
	jmp tra
tata:	jmp vvod1			
tra:	ret
	endp

print:
	
	mov ebx,0ah			
	xor cx,cx			

divloop:
	xor edx,edx			
	div ebx				
	add edx,'0'			
	push edx			
	inc cx				
	test eax,eax			
	jnz divloop			
restore:
	pop eax				
	mov edx,eax			 
	mov ah,2			
	int 21h				
	dec cx				
	cmp cx,0			
	jne restore			
	ret
	endp

endstrd:
	mov dx,offset endstr			
	mov ah,9			
	int 21h				
	ret
	endp
ret
message1 db "PRESS",0dh,0ah
	 db "+ for add. a+b.",0dh,0ah
	 db "- for sub. a-b",0dh,0ah
	 db "/ for div. a/b",0dh,0ah
	 db "* for mul. a*b",0dh,0ah
	 db "q for quit",0dh,0ah,'$'	
message2 db "Press any key",0dh,0ah,'$'
aaaa	 db "a=",'$'
bbbb	 db "b=",'$'
endstr	 db 0dh,0ah,'$'	 
enderes  db ' ',0dh,0ah,'$'	
bufer 	 db 5				
blength  db ?				
bconteg:				
hexstring equ bconteg
end start