// Javascript
'use strict';

const e = React.createElement;

Object.defineProperty(Array.prototype, 'chunk', {
    value: function (chunkSize) {
        var R = [];
        for (var i = 0; i < this.length; i += chunkSize)
            R.push(this.slice(i, i + chunkSize));
        return R;
    }
});

function renderColumn(column) {
    return <div class="gallery__column">
        {column.map(image =>
            <a class="gallery__link" href={image.path} data-lightbox="example-set" data-title="Click the right half of the image to move forward.">
                <figure class="gallery__thumb">
                    <img class="gallery__image" src={image.thumbnailPath} alt="Alina Text" />
                    <figcaption class="gallery__caption">{image.name}</figcaption>
                </figure>
            </a>
        )} </div>;
}

class MyComponent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      error: null,
      isLoaded: false,
      images: []
    };
  }

  componentDidMount() {
    fetch("api/images/list")
      .then(res => res.json())
      .then(
        (result) => {
          this.setState({
            isLoaded: true,
            images: result
          });
        },
        // Note: it's important to handle errors here
        // instead of a catch() block so that we don't swallow
        // exceptions from actual bugs in components.
        (error) => {
          this.setState({
            isLoaded: true,
            error
          });
        }
      )
  }
    
  render() {
    const { error, isLoaded, images } = this.state;
    if (error) {
      return <div>Error: {error.message}</div>;
    } else if (!isLoaded) {
      return <div>Loading...</div>;
    } else {
      var chunks = images.chunk(images.length / 3);
      return (
              chunks.map(image_array => renderColumn(image_array))
      );
    }
  }
}

const domContainer = document.querySelector('#image_container');
const root = ReactDOM.createRoot(domContainer);
root.render(e(MyComponent));