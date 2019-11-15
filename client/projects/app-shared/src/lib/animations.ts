import {
    animate, style, keyframes, animation, AnimationReferenceMetadata
} from '@angular/animations';

export const slideInRight: AnimationReferenceMetadata = animation([
    animate('.3s', keyframes([
        style({
            offset: 0,
            transform: 'translate3d(100%, 0, 0)',
            visibility: 'visible'
        }),
        style({
            offset: 1,
            transform: 'translate3d(0, 0, 0)'
        })
    ]))
]);

export const slideOutRight: AnimationReferenceMetadata = animation([
    animate('.3s', keyframes([
        style({
            offset: 0,
            transform: 'translate3d(0, 0, 0)'
        }),
        style({
            offset: 1,
            visibility: 'hidden',
            transform: 'translate3d(100%, 0, 0)'
        })
    ]))
]);

