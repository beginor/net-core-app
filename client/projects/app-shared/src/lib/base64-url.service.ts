import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class Base64UrlService {

    public encode(input: string): string {
        let result = btoa(input);
        result = result.split('=')[0];
        result = result.replace('+', '-');
        result = result.replace('/', '_');
        return result;
    }

    public decode(input: string): string {
        let output = input;
        output = output.replace('-', '+');
        output = output.replace('_', '/');
        switch (output.length % 4) {
            case 0:
                break;
            case 2:
                output += '==';
                break;
            case 3:
                output += '=';
                break;
            default:
                throw new Error(`Invalid base64 string ${input}`);
        }
        return atob(output);
    }
}
