# Project Title: Campus Swap(?) Dorm Shop(?)

## Well Written Problem Description.
> when i arrived at campus, the rooms literally had nothing on them, and there wasnt any 
affordable stuff in campus to get some basic things, despite surely someone living in the 
room before so i was thinking, maybe a small software that can help like, students that 
are leaving and have to leave some stuff behind to be able to give them to campus to sell 
at a low price and the student gets a small part of that too. like for example you leave 
back and you cant carry a screen you bought here, instead of selling it at the first pawn 
shop you find you can give it to the university and there would be a small shop for stuff 
like that that ohter students have left to make the moving in easier

In university housing, students living away from home struggle to buy and sell dorm 
furnishings due to quick semester changeovers and a lack of transportation access. This 
causes students to buy and sell items at pawn shops and online marketplaces where sellers 
are trying to make money, not help students. This adds stress to students, who are already 
financially burdened and are typically on a short timeline to move in or out before the
next semester begins. The goal of this project is to create an online marketplace where 
students can buy and sell housing furnishings to other students at the same campus, 
eliminating transportation issues and allowing students to support other students. 

### Two User Roles
1. Buyer
    #### Main Functional Requirements
    1. Buyer can view/search items. (could be category based, conditions, price)
    2. Buyer can communicate with the seller for more details about the interested listing.
    3. Buyer can receive email alerts if seller responds to their messages.
    #### Main Quality Requirements
    1. Robust: Buyer's filter must be consistent after viewing an item i.e. buyer shouldn't have to filter items again after viewing an item from previous search/filter.
    2. Time-efficiency: when a filter is applied on the listings, the page must populate within 2 seconds.
    3.  Correctness: If seller marks an item as sold while buyer is viewing an item, this should  reflect immediately on buyer's end without having to refresh.
    
2. Seller
    #### Main Functional Requirements
    1. Seller can create a listing. (must specify category, conditions, price)
    2. Seller can edit a listing (includes on hold, sold, other modifications to existing listing).
    3. Seller can delete a listing.
    4. Seller can receive email alerts for communication with buyers.
    
    #### Main Quality Requirements
    1. Robust: If seller misses mandatory fileds (i.e. category, conditions, price), they should get an alert.
    2. Time Efficiency: Seller must be able to post items within 5 minutes (in average). Conditions are name of the item, a picture, short description, conditiona and price.
    3. Correctness: If seller decides to make modifications to the listing after initial post, the data must accurately reflect immediately.



