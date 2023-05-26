
var cartVue = new Vue({
    el: '#cartVue',
    data: {
        rooms: [],
    },
    mounted: function () {
        var _this = this;
        _this.freshCart();
    },
    methods: {
        freshCart: function () {
            var _this = this;
            _this.rooms = _this.getCartList('cartItem');
        },
        getRooms: function () {
            var _this = this;
            _this.rooms = [];
            axios.post(`Cart/GetRooms`)
                .then(response => {
                    //console.log(response.data);
                    _this.rooms = response.data;
                })
                .catch();
        },
        addToCart: function (room) {
            var _this = this;
            var cookieName = 'cartItem';
            //將房間物件加入cookie
            var cartList = _this.getCartList(cookieName);
            cartList.push(room);
            //console.log(cartList);
            _this.rooms = cartList;
            _this.setCookie(cookieName, cartList, 365);
        },
        removeFromCart: function (room) {
            var _this = this;
            var cookieName = 'cartItem';
            var cartList = _this.getCartList(cookieName);
            var idx = cartList.findIndex(r => r.roomId == room.roomId);
            cartList.splice(idx, 1);
            _this.rooms = cartList;
            _this.setCookie(cookieName, cartList, 365);

        },
        setCookie: function (name, value, days) {
            var _this = this;
            var expires = "";
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toUTCString();
            }
            document.cookie = name + "=" + encodeURIComponent(JSON.stringify(value)) + expires + "; path=/";
        },
        getCartList: function (name) {
            var cartList = [];

            var cookies = document.cookie.split(';');

            for (i = 0; i < cookies.length; i++) {
                var cookie = cookies[i];
                if (cookie.startsWith(name + '=')) {
                    cartList = JSON.parse(decodeURIComponent(cookie.substring(name.length + 1)));
                }
            }
            return cartList;
        },
    },
});
