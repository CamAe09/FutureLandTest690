using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void GetRequest<T>(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(GetRequestCor<T>(sdkType, url, headers, onComplete, onError, errorsToCheck));
		}

		public void GetRequest<T>(SdkType sdkType, string url, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(GetRequestCor<T>(sdkType, url, headers, onComplete, onError, errorsToCheck));
		}

		public void GetRequest<T>(SdkType sdkType, string url, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(GetRequestCor<T>(sdkType, url, headers, onComplete, onError, errorsToCheck));
		}

		public void GetRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(GetRequestCor(sdkType, url, headers, onComplete, onError, errorsToCheck));
		}

		private IEnumerator GetRequestCor<T>(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders = null, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = UnityWebRequest.Get(url);
			AttachHeadersToRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator GetRequestCor(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders = null, Action onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = UnityWebRequest.Get(url);
			AttachHeadersToRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
	}
}
