$(window).on("scroll", function () {
	var scrollTop = $(window).scrollTop();
	var path = window.location.href;
	if (path === 'https://localhost:44372/') {
		if (scrollTop >= 100) {
			$('body').addClass('fixed-header')
		} else {
			$('body').removeClass('fixed-header')
		}
	}
	else {
		$('body').addClass('fixed-header')
	}
})


$('.navbar-toggler').on('click', function () {
	$('body').addClass('fixed-header')
});

$('.btn-close').on('click', function () {
	$('body').removeClass('fixed-header')
});

function replaceSpecialCharacters() {
	let html = document.querySelector("html");
	let cleanHTML = html.innerHTML.replace(/[\u200B-\u200D\uFEFF]/gim, '');
	html.innerHTML = cleanHTML;
}

function setupTyped() {
	new Typed('#typed-id-0', {
		strings: ['Khám phá thêm nhiều trải nghiệm mới'],
		typeSpeed: 100,
	});
	new Typed('#typed-id-1', {
		strings: ['Bộ sưu tập những câu chuyện sống tại M Village'],
		typeSpeed: 100,
	});
	new Typed('#typed-id-2', {
		strings: ['Khám phá thêm nhiều trải nghiệm mới'],
		typeSpeed: 100,
	});
}

// Transition onload
window.onload = () => {
	if ($(".transition").hasClass("is-active")) {
		setTimeout(() => {
			$(".transition").removeClass("is-active");
		}, 300);

	}
};

$(document).ready(function () {
	//replaceSpecialCharacters();

	var path = window.location.href;
	if (path !== 'https://localhost:44372/') {
		$('body').addClass('fixed-header')
	}

	$('.owl-one').owlCarousel({
		animateOut: 'fadeOut',
		animateIn: 'flipInX',
		items: 1,
		loop: false,
		autoplay: true,
		autoplaySpeed: 10000,
		autoplayTimeout: 10000,
	});

	$('.owl-two').owlCarousel({
		animateOut: 'slideOutDown',
		animateIn: 'flipInX',
		loop: false,
		autoplay: false,
		margin: 10,
		dots: false,
		autoplaySpeed: 500,
		autoplayTimeout: 5000,
		nav: true,
		navText: ["<div class='nav-btn prev-slide'></div>", "<div class='nav-btn next-slide'></div>"],
		responsive: {
			0: {
				items: 1
			},
			// breakpoint from 480 up
			900: {
				items: 3
			},
		}
	});

	$('.owl-three').owlCarousel({
		animateOut: 'slideOutDown',
		animateIn: 'flipInX',
		items: 1,
		loop: false,
		autoplay: false,
		autoplaySpeed: 500,
		autoplayTimeout: 5000,
		dots: false,
		nav: true,
		navText: ["<div class='nav-btn prev-slide'></div>", "<div class='nav-btn next-slide'></div>"],
	});

	$('.owl-four').owlCarousel({
		animateOut: 'slideOutDown',
		animateIn: 'flipInX',
		items: 1,
		loop: true,
		autoplay: true,
		autoplaySpeed: 500,
		autoplayTimeout: 5000,
		dots: false,
		nav: true,
		navText: ["<div class='nav-btn prev-slide'></div>", "<div class='nav-btn next-slide'></div>"],
	});

	if (window.matchMedia("(max-width: 767px)").matches) {
		// The screen width is 767px or less, so it's a mobile device
		console.log("Mobile screen detected");

		$('.owl-two').owlCarousel({
			animateOut: 'slideOutDown',
			animateIn: 'flipInX',
			loop: true,
			autoplay: true,
			margin: 10,
			nav: true, stagePadding: 100,
			autoplaySpeed: 500,
			autoplayTimeout: 5000,
			responsive: {
				0: {
					items: 1
				},
				// breakpoint from 480 up
				900: {
					items: 3
				},
			}
		});
	} else {
		// The screen width is more than 767px, so it's not a mobile device
		console.log("Not a mobile screen");
	}


});


