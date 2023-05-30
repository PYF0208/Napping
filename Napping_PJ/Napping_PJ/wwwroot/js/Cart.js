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
        getRooms: async function () {
            var _this = this;
            _this.rooms = [];
            await axios.post(`Cart/GetRooms`)
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
            var jsonStr = JSON.stringify(value);
            var encodeStr = encodeURIComponent(jsonStr);
            _this.$cookies.set(name, encodeStr, { expires: days });
        },
        getCartList: function (name) {
            var _this = this;
            var cartList = [];

            var cookies = document.cookie.split(';');

            var encodedValue = _this.$cookies.get(name);
            if (encodedValue) {
                var decodedValue = decodeURIComponent(encodedValue);
                cartList = JSON.parse(decodedValue);
            }
            return cartList;
        },
    },
    watch: {
        'rooms': function (newValue) {
            //console.log('Cookie發生變化:', newValue);
        }
    },
});
