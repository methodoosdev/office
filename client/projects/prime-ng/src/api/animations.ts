// import the required animation functions from the angular animations module
import { trigger, animate, transition, style, state, group } from '@angular/animations';

export const fadeInAnimation =
    // trigger name for attaching this animation to an element using the [@triggerName] syntax
    trigger('fadeInAnimation', [

        // route 'enter' transition
        transition(':enter', [

            // css styles at start of transition
            style({ opacity: 0 }),

            // animation and styles at end of transition
            animate('.3s', style({ opacity: 1 }))
        ]),
    ]);

export const stateAnimation =
    // trigger name for attaching this animation to an element using the [@triggerName] syntax
    trigger('animationState', [
        state('active', style({ opacity: 1 })),
        transition('* => active', [
            style({ opacity: 0 }),
            animate('250ms ease-in')
        ])
    ]);

export const slideInAnimation =
    trigger('slideInOut', [
        state('hidden', style({
            height: '0px'
        })),
        state('visible', style({
            height: '*'
        })),
        transition('visible => hidden', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)')),
        transition('hidden => visible', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
    ]);

export const slideInOutAnimation =
    trigger('slideInOut', [
        state('in', style({
            'height': '*', 'opacity': '1', 'visibility': 'visible',
        })),
        state('out', style({
            'height': '0px', 'opacity': '0', 'visibility': 'hidden',
        })),
        transition('in => out', [group([
            animate('400ms ease-in-out', style({
                'opacity': '0',
            })),
            animate('600ms ease-in-out', style({
                'height': '0px',
            })),
            animate('700ms ease-in-out', style({
                'visibility': 'hidden',
            })),
        ],
        )]),
        transition('out => in', [group([
            animate('1ms ease-in-out', style({
                'visibility': 'visible',
            })),
            animate('600ms ease-in-out', style({
                'height': '*',
            })),
            animate('800ms ease-in-out', style({
                'opacity': '1',
            })),
        ],
        )]),
    ]);