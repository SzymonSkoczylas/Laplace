.code
BitmapProc proc

; Input:
; rcx - Pointer to the beginning of the bitmap

mov rdi, rcx;

mov byte ptr [rdi+0], 255
mov byte ptr [rdi+3], 255
mov byte ptr [rdi+6], 255

ret

BitmapProc endp
end