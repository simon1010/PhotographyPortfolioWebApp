// Javascript
'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ('value' in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

function _inherits(subClass, superClass) { if (typeof superClass !== 'function' && superClass !== null) { throw new TypeError('Super expression must either be null or a function, not ' + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var e = React.createElement;

Object.defineProperty(Array.prototype, 'chunk', {
  value: function value(chunkSize) {
    var R = [];
    for (var i = 0; i < this.length; i += chunkSize) R.push(this.slice(i, i + chunkSize));
    return R;
  }
});

function renderColumn(column) {
  return React.createElement(
    'div',
    { 'class': 'gallery__column' },
    column.map(function (image) {
      return React.createElement(
        'a',
        { 'class': 'gallery__link', href: image.path, 'data-lightbox': 'example-set', 'data-title': 'Click the right half of the image to move forward.' },
        React.createElement(
          'figure',
          { 'class': 'gallery__thumb' },
          React.createElement('img', { 'class': 'gallery__image', src: image.thumbnailPath, alt: 'Alina Text' }),
          React.createElement(
            'figcaption',
            { 'class': 'gallery__caption' },
            image.name
          )
        )
      );
    }),
    ' '
  );
}

var MyComponent = (function (_React$Component) {
  _inherits(MyComponent, _React$Component);

  function MyComponent(props) {
    _classCallCheck(this, MyComponent);

    _get(Object.getPrototypeOf(MyComponent.prototype), 'constructor', this).call(this, props);
    this.state = {
      error: null,
      isLoaded: false,
      images: []
    };
  }

  _createClass(MyComponent, [{
    key: 'componentDidMount',
    value: function componentDidMount() {
      var _this = this;

      fetch("api/images/list").then(function (res) {
        return res.json();
      }).then(function (result) {
        _this.setState({
          isLoaded: true,
          images: result
        });
      },
      // Note: it's important to handle errors here
      // instead of a catch() block so that we don't swallow
      // exceptions from actual bugs in components.
      function (error) {
        _this.setState({
          isLoaded: true,
          error: error
        });
      });
    }
  }, {
    key: 'render',
    value: function render() {
      var _state = this.state;
      var error = _state.error;
      var isLoaded = _state.isLoaded;
      var images = _state.images;

      if (error) {
        return React.createElement(
          'div',
          null,
          'Error: ',
          error.message
        );
      } else if (!isLoaded) {
        return React.createElement(
          'div',
          null,
          'Loading...'
        );
      } else {
        var chunks = images.chunk(images.length / 3);
        return chunks.map(function (image_array) {
          return renderColumn(image_array);
        });
      }
    }
  }]);

  return MyComponent;
})(React.Component);

var domContainer = document.querySelector('#image_container');
var root = ReactDOM.createRoot(domContainer);
root.render(e(MyComponent));

